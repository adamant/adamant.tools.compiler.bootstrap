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

        public override bool Enter(ISyntax syntax, ISyntaxTraversal traversal)
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
                    traversal.Walk(foreachExpression.TypeSyntax);
                    traversal.Walk(foreachExpression.InExpression);
                    scopes.Push(
                        new NestedScope(ContainingScope, foreachExpression.Yield(), Enumerable.Empty<ISymbol>()));
                    traversal.Walk(foreachExpression.Block);
                    scopes.Pop();
                    return false;
                case IBodyOrBlockSyntax bodyOrBlock:
                {
                    var scopeDepth = scopes.Count;

                    foreach (var statement in bodyOrBlock.Statements)
                        traversal.Walk(statement);

                    // Remove any scopes created by variable declarations
                    while (scopes.Count > scopeDepth) scopes.Pop();

                    return false;
                }
            }

            return true;
        }
    }
}
