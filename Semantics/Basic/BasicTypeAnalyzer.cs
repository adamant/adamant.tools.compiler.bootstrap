using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    /// <summary>
    /// Analyzes an <see cref="ITypeSyntax" /> to evaluate which type it refers to.
    /// </summary>
    public class BasicTypeAnalyzer
    {
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public BasicTypeAnalyzer(CodeFile file, Diagnostics diagnostics)
        {
            this.file = file;
            this.diagnostics = diagnostics;
        }

        [return: NotNullIfNotNull("typeSyntax")]
        public DataType? Evaluate(ITypeSyntax? typeSyntax)
        {
            switch (typeSyntax)
            {
                default:
                    throw ExhaustiveMatch.Failed(typeSyntax);
                case null:
                    return null;
                case ITypeNameSyntax typeName:
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
                case IReferenceLifetimeTypeSyntax referenceLifetime:
                {
                    var type = Evaluate(referenceLifetime.ReferentType);
                    if (type == DataType.Unknown)
                        return DataType.Unknown;
                    throw new NotImplementedException();
                    //var lifetime = EvaluateLifetime(referenceLifetime.Lifetime);
                    if (type is ReferenceType referenceType)
                        //referenceLifetime.NamedType = referenceType.WithLifetime(lifetime);
                        throw new NotImplementedException();
                    else
                        referenceLifetime.NamedType = DataType.Unknown;
                    break;
                }
                case IOptionalTypeSyntax optionalType:
                {
                    var referent = Evaluate(optionalType.Referent);
                    return new OptionalType(referent);
                }
                case IMutableTypeSyntax mutableType:
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
            }

            return typeSyntax.NamedType ?? throw new InvalidOperationException();
        }

        //public static Lifetime EvaluateLifetime(SimpleName lifetimeName)
        //{
        //    if (lifetimeName.IsSpecial)
        //    {
        //        if (lifetimeName == SpecialName.Owned)
        //            return Lifetime.Owned;
        //        if (lifetimeName == SpecialName.Forever)
        //            return Lifetime.Forever;
        //        throw new InvalidOperationException($"Invalid special lifetime name: {lifetimeName.Text}");
        //    }

        //    return new NamedLifetime(lifetimeName.Text);
        //}
    }
}
