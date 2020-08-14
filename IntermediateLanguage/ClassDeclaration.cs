using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class ClassDeclaration : Declaration
    {
        public FixedList<Declaration> Members { get; }
        public new ObjectTypeSymbol Symbol { get; }

        public ClassDeclaration(
            MaybeQualifiedName name,
            ObjectTypeSymbol symbol,
            FixedList<Declaration> members)
            : base(false, name, symbol)
        {
            Symbol = symbol;
            Members = members;
        }
    }
}
