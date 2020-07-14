using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.FST.Walkers
{
    internal class VariableDeclarationsCollector : SyntaxWalker
    {
        private readonly List<IBindingSymbol> symbols = new List<IBindingSymbol>();

        public FixedList<IBindingSymbol> Symbols => symbols.ToFixedList();

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IVariableDeclarationStatementSyntax exp:
                    symbols.Add(exp);
                    break;
                case IForeachExpressionSyntax exp:
                    symbols.Add(exp);
                    break;
                case ITypeSyntax _:
                    return;
                case IDeclarationSyntax _:
                    throw new InvalidOperationException($"Can't get variable declarations of {syntax.GetType().Name}");
            }

            WalkChildren(syntax);
        }
    }
}
