using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public class UnknownSymbol : ISymbol
    {
        #region Singleton
        public static readonly UnknownSymbol Instance = new UnknownSymbol();

        private UnknownSymbol() { }
        #endregion

        public Name FullName => SpecialName.Unknown;
        public SimpleName LookupByName => SpecialName.Unknown;
        public DataType Type => DataType.Unknown;
        public SymbolSet ChildSymbols => SymbolSet.Empty;
    }
}
