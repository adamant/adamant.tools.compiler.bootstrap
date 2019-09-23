using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    /// <summary>
    /// Analyzes a <see cref="TypeSyntax" /> to evaluate which type it refers to.
    /// </summary>
    public class BasicTypeAnalyzer
    {
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;
        private readonly BasicStatementAnalyzer statementAnalyzer;

        public BasicTypeAnalyzer(
            CodeFile file,
            Diagnostics diagnostics,
            BasicStatementAnalyzer statementAnalyzer)
        {
            this.file = file;
            this.diagnostics = diagnostics;
            this.statementAnalyzer = statementAnalyzer;
        }

        public DataType Evaluate(TypeSyntax typeSyntax)
        {
            switch (typeSyntax)
            {
                default:
                    throw ExhaustiveMatch.Failed(typeSyntax);
                case TypeNameSyntax typeName:
                {
                    var symbols = typeName.LookupInContainingScope().OfType<ITypeSymbol>().ToFixedList();
                    switch (symbols.Count)
                    {
                        case 0:
                            diagnostics.Add(NameBindingError.CouldNotBindName(file, typeName.Span));
                            typeName.ReferencedSymbol = UnknownSymbol.Instance;
                            typeSyntax.NamedType = DataType.Unknown;
                            break;
                        case 1:
                            var symbol = symbols.Single();
                            typeName.ReferencedSymbol = symbol;
                            typeName.NamedType = symbol.DeclaresType;
                            break;
                        default:
                            diagnostics.Add(NameBindingError.AmbiguousName(file, typeName.Span));
                            typeName.ReferencedSymbol = UnknownSymbol.Instance;
                            typeName.NamedType = DataType.Unknown;
                            break;
                    }
                    break;
                }
                case ReferenceLifetimeSyntax referenceLifetime:
                {
                    var type = Evaluate(referenceLifetime.ReferentType);
                    if (type == DataType.Unknown)
                        return DataType.Unknown;
                    var lifetime = EvaluateLifetime(referenceLifetime.Lifetime);
                    if (type is ReferenceType referenceType)
                        referenceLifetime.NamedType = referenceType.WithLifetime(lifetime);
                    else
                        referenceLifetime.NamedType = DataType.Unknown;
                    break;
                }
                //case UnaryExpressionSyntax unaryOperatorExpression:
                //    switch (unaryOperatorExpression.Operator)
                //    {
                //        case UnaryOperator.Question:
                //            //unaryOperatorExpression.Type = DataType.Type;
                //            var referent = Check(unaryOperatorExpression.Operand);
                //            return new OptionalType(referent);
                //        default:
                //            // TODO evaluate to type
                //            return DataType.Unknown;
                //    }
                case MutableTypeSyntax mutableType:
                {
                    var type = Evaluate(mutableType.Referent);
                    switch (type)
                    {
                        case UserObjectType objectType when objectType.DeclaredMutable:
                            mutableType.NamedType = objectType.AsMutable();
                            break;
                        default:
                            mutableType.NamedType = DataType.Unknown;
                            break;
                    }
                    break;
                }
                case SelfTypeSyntax selfType:
                    throw new NotImplementedException();
            }

            return typeSyntax.NamedType;
        }

        public static Lifetime EvaluateLifetime(SimpleName lifetimeName)
        {
            if (lifetimeName.IsSpecial)
            {
                if (lifetimeName == SpecialName.Owned)
                    return Lifetime.Owned;
                if (lifetimeName == SpecialName.Forever)
                    return Lifetime.Forever;
                throw NonExhaustiveMatchException.For(lifetimeName.Text);
            }

            return new NamedLifetime(lifetimeName.Text);
        }
    }
}
