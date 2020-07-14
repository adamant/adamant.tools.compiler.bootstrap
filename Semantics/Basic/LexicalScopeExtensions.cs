using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    internal static class LexicalScopeExtensions
    {
        public static ITypeSymbol GetSymbolForType(this LexicalScope containingScope, DataType type)
        {
            return type switch
            {
                UnknownType _ => UnknownSymbol.Instance,
                ObjectType objectType =>
                        containingScope
                        .LookupInGlobalScope(objectType.FullName)
                        .OfType<ITypeSymbol>()
                        .Single(),
                SizedIntegerType integerType =>
                        containingScope
                        .LookupInGlobalScope(integerType.Name)
                        .OfType<ITypeSymbol>()
                        .Single(),
                UnsizedIntegerType integerType =>
                        containingScope
                        .LookupInGlobalScope(integerType.Name)
                        .OfType<ITypeSymbol>()
                        .Single(),
                _ => throw new NotImplementedException(
                    $"{nameof(GetSymbolForType)} not implemented for {type.GetType().Name}")
            };
        }
    }
}
