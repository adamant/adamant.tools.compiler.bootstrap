using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    public class SyntaxScopesBuilder : SyntaxWalker
    {
        private readonly Stack<LexicalScope> scopes = new Stack<LexicalScope>();
        private LexicalScope ContainingScope => scopes.Peek();

        public SyntaxScopesBuilder(LexicalScope containingScope)
        {
            scopes.Push(containingScope);
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    // Each variable declaration effectively starts a new scope after it, this
                    // ensures a lookup returns the last declaration
                    scopes.Push(new NestedScope(ContainingScope, variableDeclaration.Yield(), Enumerable.Empty<ISymbol>()));
                    break;
                case INameExpressionSyntax nameExpression:
                    nameExpression.ContainingScope = ContainingScope;
                    break;
                case ITypeNameSyntax typeName:
                    typeName.ContainingScope = ContainingScope;
                    break;
                case IForeachExpressionSyntax foreachExpression:
                    Walk(foreachExpression.TypeSyntax);
                    Walk(foreachExpression.InExpression);
                    scopes.Push(new NestedScope(ContainingScope, foreachExpression.Yield(), Enumerable.Empty<ISymbol>()));
                    Walk(foreachExpression.Block);
                    scopes.Pop();
                    return;
                case IBodyOrBlockSyntax bodyOrBlock:
                {
                    var scopeDepth = scopes.Count;

                    foreach (var statement in bodyOrBlock.Statements)
                        Walk(statement);

                    // Remove any scopes created by variable declarations
                    while (scopes.Count > scopeDepth) scopes.Pop();

                    return;
                }
            }

            WalkChildren(syntax);
        }
    }
}
