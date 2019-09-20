using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class Parameter : IBindingSymbol
    {
        public Name FullName { get; }

        public SimpleName Name => FullName.UnqualifiedName;

        public SymbolSet ChildSymbols => SymbolSet.Empty;

        public DataType Type { get; }

        public bool IsMutableBinding => false;

        public Parameter(Name fullName, DataType type)
        {
            FullName = fullName;
            Type = type;
        }
    }
}
