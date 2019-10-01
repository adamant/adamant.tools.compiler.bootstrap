using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    public class ExpressionScopesBuilder : ExpressionVisitor<LexicalScope>
    {
        public override void VisitBlock(BlockSyntax block, LexicalScope containingScope)
        {
            if (block == null)
                return;

            foreach (var statement in block.Statements)
            {
                VisitStatement(statement, containingScope);
                // Each variable declaration effectively starts a new scope after it, this
                // ensures a lookup returns the last declaration
                if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                    containingScope = new NestedScope(containingScope, variableDeclaration.Yield(),
                        Enumerable.Empty<ISymbol>());
            }
        }

        public override void VisitName(
            NameSyntax name,
            LexicalScope containingScope)
        {
            name.ContainingScope = containingScope;
        }

        public override void VisitForeachExpression(ForeachExpressionSyntax foreachExpression, LexicalScope containingScope)
        {
            VisitType(foreachExpression.TypeSyntax, containingScope);
            VisitExpression(foreachExpression.InExpression, containingScope);
            containingScope = new NestedScope(containingScope, foreachExpression.Yield(), Enumerable.Empty<ISymbol>());
            VisitExpression(foreachExpression.Block, containingScope);
        }

        public override void VisitTypeName(ITypeNameSyntax typeName, LexicalScope containingScope)
        {
            typeName.ContainingScope = containingScope;
        }
    }
}
