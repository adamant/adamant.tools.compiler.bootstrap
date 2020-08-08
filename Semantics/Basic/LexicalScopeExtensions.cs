using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    internal static class LexicalScopeExtensions
    {
        public static ITypeMetadata GetMetadataForType(this Scope containingScope, DataType type)
        {
            return type switch
            {
                UnknownType _ => UnknownMetadata.Instance,
                ObjectType objectType =>
                        containingScope
                        .LookupMetadataInGlobalScope(objectType.FullName)
                        .OfType<ITypeMetadata>()
                        .Single(),
                SizedIntegerType integerType =>
                        containingScope
                        .LookupMetadataInGlobalScope(integerType.Name)
                        .OfType<ITypeMetadata>()
                        .Single(),
                UnsizedIntegerType integerType =>
                        containingScope
                        .LookupMetadataInGlobalScope(integerType.Name)
                        .OfType<ITypeMetadata>()
                        .Single(),
                _ => throw new NotImplementedException(
                    $"{nameof(GetMetadataForType)} not implemented for {type.GetType().Name}")
            };
        }
    }
}
