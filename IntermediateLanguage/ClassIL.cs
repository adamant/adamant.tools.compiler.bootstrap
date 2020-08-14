using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class ClassIL : DeclarationIL
    {
        public FixedList<DeclarationIL> Members { get; }
        public new ObjectTypeSymbol Symbol { get; }

        public ClassIL(ObjectTypeSymbol symbol, FixedList<DeclarationIL> members)
            : base(false, symbol)
        {
            Symbol = symbol;
            Members = members;
        }
    }
}
