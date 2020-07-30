using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    internal static class LexicalScopeExtensions
    {
        public static ITypeMetadata GetSymbolForType(this LexicalScope containingScope, DataType type)
        {
            return type switch
            {
                UnknownType _ => UnknownMetadata.Instance,
                ObjectType objectType =>
                        containingScope
                        .LookupInGlobalScope(objectType.FullName)
                        .OfType<ITypeMetadata>()
                        .Single(),
                SizedIntegerType integerType =>
                        containingScope
                        .LookupInGlobalScope(integerType.Name)
                        .OfType<ITypeMetadata>()
                        .Single(),
                UnsizedIntegerType integerType =>
                        containingScope
                        .LookupInGlobalScope(integerType.Name)
                        .OfType<ITypeMetadata>()
                        .Single(),
                _ => throw new NotImplementedException(
                    $"{nameof(GetSymbolForType)} not implemented for {type.GetType().Name}")
            };
        }
    }
}
