using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    /// <summary>
    /// Analyzes an <see cref="ITypeSyntax" /> to evaluate which type it refers to.
    /// </summary>
    public class TypeResolver
    {
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public TypeResolver(CodeFile file, Diagnostics diagnostics)
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
                    var symbolPromises = typeName.LookupInContainingScope().Select(p => p.As<TypeSymbol>()).NotNull().ToFixedList();
                    typeName.ReferencedSymbol.BeginFulfilling();
                    switch (symbolPromises.Count)
                    {
                        case 0:
                            diagnostics.Add(NameBindingError.CouldNotBindName(file, typeName.Span));
                            typeName.ReferencedSymbol.Fulfill(null);
                            typeName.NamedType = DataType.Unknown;
                            break;
                        case 1:
                            var symbol = symbolPromises.Single().Result;
                            typeName.ReferencedSymbol.Fulfill(symbol);
                            typeName.NamedType = symbol.DeclaresDataType;
                            break;
                        default:
                            diagnostics.Add(NameBindingError.AmbiguousName(file, typeName.Span));
                            typeName.ReferencedSymbol.Fulfill(null);
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
                        referenceCapability.NamedType = referenceType.To(referenceCapability.Capability);
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
