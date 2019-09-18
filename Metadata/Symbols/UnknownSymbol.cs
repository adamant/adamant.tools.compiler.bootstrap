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

        // We don't know what this is, so it might be mutable (fewer errors this way)
        public bool MutableBinding => true;
        public Name FullName => SpecialName.Unknown;
        public DataType Type => DataType.Unknown;
        DataType ISymbol.DeclaresType => null;
        public SymbolSet ChildSymbols => SymbolSet.Empty;
    }
}
