using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    public class UnknownSymbol : ITypeSymbol, IFunctionSymbol, IBindingSymbol
    {
        #region Singleton
        public static readonly UnknownSymbol Instance = new UnknownSymbol();

        private UnknownSymbol() { }
        #endregion

        // We don't know what this is, so it might be mutable (fewer errors this way)
        public bool IsMutableBinding => true;
        public Name FullName => SpecialName.Unknown;
        public DataType Type => DataType.Unknown;
        public DataType DeclaresType => DataType.Unknown;
        public SymbolSet ChildSymbols => SymbolSet.Empty;

        public IEnumerable<IBindingSymbol> Parameters => Enumerable.Empty<IBindingSymbol>();

        public DataType ReturnType => DataType.Unknown;
    }
}
