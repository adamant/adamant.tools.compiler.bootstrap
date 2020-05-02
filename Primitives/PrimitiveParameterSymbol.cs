using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

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
