using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    public class SyntaxScopesBuilder : SyntaxWalker<LexicalScope>
    {
        protected override void WalkNonNull(ISyntax syntax, LexicalScope containingScope)
        {
            switch (syntax)
            {
                case IHasContainingScope hasContainingScope:
                    hasContainingScope.ContainingScope = containingScope;
                    break;
                case IForeachExpressionSyntax foreachExpression:
                    Walk(foreachExpression.TypeSyntax, containingScope);
                    Walk(foreachExpression.InExpression, containingScope);
                    containingScope = new NestedScope(containingScope, foreachExpression.Yield(), Enumerable.Empty<ISymbol>());
                    Walk(foreachExpression.Block, containingScope);
                    return;
                case IBodyOrBlockSyntax bodyOrBlock:
                    foreach (var statement in bodyOrBlock.Statements)
                    {
                        Walk(statement, containingScope);
                        // Each variable declaration effectively starts a new scope after it, this
                        // ensures a lookup returns the last declaration
                        if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                            containingScope = new NestedScope(
                                                containingScope,
                                                variableDeclaration.Yield(),
                                                Enumerable.Empty<ISymbol>());
                    }
                    return;
            }

            WalkChildren(syntax, containingScope);
        }
    }
}
