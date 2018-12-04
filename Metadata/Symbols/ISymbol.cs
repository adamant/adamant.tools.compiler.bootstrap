using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public interface ISymbol
    {
        Name FullName { get; }
        /// <summary>
        /// The name this item is looked up by. For example, variables don't have
        /// the number on them and get/set use the property name.
        /// </summary>
        SimpleName LookupByName { get; }
        DataType Type { get; }
        SymbolSet ChildSymbols { get; }
    }
}
