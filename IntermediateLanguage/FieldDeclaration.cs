using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class FieldDeclaration : Declaration
    {
        public bool IsMutableBinding { get; }
        public DataType DataType { get; }
        public new FieldSymbol Symbol { get; }

        public FieldDeclaration(bool isMutableBinding, MaybeQualifiedName fullName, DataType type, FieldSymbol symbol)
            : base(true, fullName, symbol)
        {
            IsMutableBinding = isMutableBinding;
            DataType = type;
            Symbol = symbol;
        }
    }
}
