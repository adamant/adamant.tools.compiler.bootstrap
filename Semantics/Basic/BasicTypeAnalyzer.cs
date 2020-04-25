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
                case ICapabilityTypeSyntax referenceCapability:
                {
                    var type = Evaluate(referenceCapability.ReferentType);
                    if (type == DataType.Unknown)
                        return DataType.Unknown;
                    if (type is ReferenceType referenceType)
                        referenceCapability.NamedType = referenceType.WithCapability(referenceCapability.Capability);
                    else
                        referenceCapability.NamedType = DataType.Unknown;
                    break;
                }
                case IOptionalTypeSyntax optionalType:
                {
                    var referent = Evaluate(optionalType.Referent);
                    return optionalType.NamedType = new OptionalType(referent);
                }
            }

            return typeSyntax.NamedType ?? throw new InvalidOperationException();
        }
    }
}
