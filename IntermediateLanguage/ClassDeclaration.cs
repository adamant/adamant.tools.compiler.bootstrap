using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class ClassDeclaration : Declaration
    {
        public FixedList<Declaration> Members { get; }
        public new ObjectTypeSymbol Symbol { get; }

        public ClassDeclaration(ObjectTypeSymbol symbol, FixedList<Declaration> members)
            : base(false, symbol)
        {
            Symbol = symbol;
            Members = members;
        }
    }
}
