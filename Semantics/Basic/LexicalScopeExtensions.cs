using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    internal static class LexicalScopeExtensions
    {
        public static ITypeSymbol GetSymbolForType(this LexicalScope containingScope, DataType type)
        {
            switch (type)
            {
                default:
                    throw new NotImplementedException($"{nameof(GetSymbolForType)} not implemented for {type.GetType().Name}");
                case UnknownType _:
                    return UnknownSymbol.Instance;
                case ObjectType objectType:
                    return containingScope.LookupInGlobalScope(objectType.FullName).OfType<ITypeSymbol>().Single();
                case SizedIntegerType integerType:
                    return containingScope.LookupInGlobalScope(integerType.Name).OfType<ITypeSymbol>().Single();
                case UnsizedIntegerType integerType:
                    return containingScope.LookupInGlobalScope(integerType.Name).OfType<ITypeSymbol>().Single();
            }
        }
    }
}
