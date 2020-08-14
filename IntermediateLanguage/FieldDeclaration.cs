using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class FieldDeclaration : Declaration
    {
        public bool IsMutableBinding => Symbol.IsMutableBinding;
        public DataType DataType => Symbol.DataType;
        public new FieldSymbol Symbol { get; }

        public FieldDeclaration(FieldSymbol symbol)
            : base(true, symbol)
        {
            Symbol = symbol;
        }
    }
}
