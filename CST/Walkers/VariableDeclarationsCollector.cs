using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST.Walkers
{
    internal class VariableDeclarationsCollector : SyntaxWalker
    {
        private readonly List<IBindingMetadata> metadata = new List<IBindingMetadata>();

        public FixedList<IBindingMetadata> Metadata => metadata.ToFixedList();

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IVariableDeclarationStatementSyntax exp:
                    metadata.Add(exp);
                    break;
                case IForeachExpressionSyntax exp:
                    metadata.Add(exp);
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
