using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Types;
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
                    var metadatas = typeName.LookupMetadataInContainingScope().OfType<ITypeMetadata>().ToFixedList();
                    switch (metadatas.Count)
                    {
                        case 0:
                            diagnostics.Add(NameBindingError.CouldNotBindName(file, typeName.Span));
                            typeName.ReferencedMetadata = UnknownMetadata.Instance;
                            if (typeName.NamedType is null)
                                typeName.NamedType = DataType.Unknown;
                            break;
                        case 1:
                            var metadata = metadatas.Single();
                            typeName.ReferencedMetadata = metadata;
                            if (typeName.NamedType is null)
                                typeName.NamedType = metadata.DeclaresDataType;
                            break;
                        default:
                            diagnostics.Add(NameBindingError.AmbiguousName(file, typeName.Span));
                            typeName.ReferencedMetadata = UnknownMetadata.Instance;
                            if (typeName.NamedType is null)
                                typeName.NamedType =  DataType.Unknown;
                            break;
                    }
                    break;
                }
                case ICapabilityTypeSyntax referenceCapability:
                {
                    var type = Evaluate(referenceCapability.ReferentType);
                    if (type == DataType.Unknown)
                        return DataType.Unknown;
                    if (referenceCapability.NamedType is null)
                    {
                        if (type is ReferenceType referenceType)
                            referenceCapability.NamedType = referenceType.To(referenceCapability.Capability);
                        else
                            referenceCapability.NamedType = DataType.Unknown;
                    }
                    break;
                }
                case IOptionalTypeSyntax optionalType:
                {
                    var referent = Evaluate(optionalType.Referent);
                    if (optionalType.NamedType is null)
                        optionalType.NamedType = new OptionalType(referent);
                }
                break;
            }

            return typeSyntax.NamedType ?? throw new InvalidOperationException();
        }
    }
}
