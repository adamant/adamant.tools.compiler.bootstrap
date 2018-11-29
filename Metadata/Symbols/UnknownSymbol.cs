using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public class UnknownSymbol : ISymbol
    {
        #region Singleton
        [NotNull] public static readonly UnknownSymbol Instance = new UnknownSymbol();

        private UnknownSymbol() { }
        #endregion

        public Name FullName => SpecialName.Unknown;
        public DataType Type => DataType.Unknown;
        public FixedDictionary<SimpleName, ISymbol> ChildSymbols => FixedDictionary<SimpleName, ISymbol>.Empty;
    }
}
