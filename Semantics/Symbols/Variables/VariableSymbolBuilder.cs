using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols.Variables
{
    public class VariableSymbolBuilder : SyntaxWalker<Symbol>
    {
        private VariableSymbolBuilder() { }

        public static void BuildFor(IEnumerable<IEntityDeclarationSyntax> entities)
        {
            var builder = new VariableSymbolBuilder();
            foreach (var entity in entities)
                builder.WalkNonNull(entity, entity.Symbol.Result);
        }

        protected override void WalkNonNull(ISyntax syntax, Symbol containingSymbol)
        {
            switch (syntax)
            {
                case IVariableDeclarationStatementSyntax syn:
                    break;
                case IForeachExpressionSyntax syn:
                    break;
            }

            WalkChildren(syntax, containingSymbol);
        }
    }
}
