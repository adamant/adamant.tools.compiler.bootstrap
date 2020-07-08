using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public sealed class DefaultConstructor : IFunctionSymbol
    {
        public Name FullName { get; }
        public DataType ConstructedType { get; }

        SymbolSet IParentSymbol.ChildSymbols => SymbolSet.Empty;

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Enumerable.Empty<IBindingSymbol>();
        DataType IFunctionSymbol.ReturnType => ConstructedType;

        public DefaultConstructor(ObjectType constructedType)
        {
            ConstructedType = constructedType;
            FullName = constructedType.Name.Qualify(SpecialName.New);
        }
    }
}
