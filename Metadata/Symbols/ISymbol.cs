using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public interface ISymbol
    {
        [NotNull] Name FullName { get; }
        /// <summary>
        /// The name this item is looked up by. For example, variables don't have
        /// the number on them and get/set use the property name.
        /// </summary>
        [NotNull] SimpleName LookupByName { get; }
        [NotNull] DataType Type { get; }
        [NotNull] SymbolSet ChildSymbols { get; }
    }
}
