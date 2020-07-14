using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class PrimitiveParameterSymbol : PrimitiveSymbol, IBindingSymbol
    {
        public SimpleName Name => FullName.UnqualifiedName;

        public DataType Type { get; }

        public bool IsMutableBinding => false;

        public PrimitiveParameterSymbol(Name fullName, DataType type)
            : base(fullName, SymbolSet.Empty)
        {
            Type = type;
        }
    }
}
