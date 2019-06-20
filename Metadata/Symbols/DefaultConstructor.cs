using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public class DefaultConstructor : ISymbol
    {
        bool ISymbol.MutableBinding => false;
        public Name FullName { get; }
        public DataType Type { get; }
        public SymbolSet ChildSymbols => SymbolSet.Empty;

        public DefaultConstructor(UserObjectType type)
        {
            FullName = type.Name.Qualify(SpecialName.New);
            Type = new FunctionType(Enumerable.Empty<DataType>(), type);
        }
    }
}
