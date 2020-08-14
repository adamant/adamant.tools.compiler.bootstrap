using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class ClassDeclaration : Declaration, IClassDeclaration
    {
        public new ObjectTypeSymbol Symbol { get; }

        protected ClassDeclaration(ObjectTypeSymbol symbol)
            : base(symbol)
        {
            Symbol = symbol;
        }
    }
}
