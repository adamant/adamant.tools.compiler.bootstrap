using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    // TODO Change this over to a walker
    public class ExpressionScopesBuilder : ExpressionWalker, IStatementWalker
    {
        private readonly Stack<LexicalScope> scopes = new Stack<LexicalScope>();
        private LexicalScope ContainingScope => scopes.Peek();

        public ExpressionScopesBuilder(LexicalScope containingScope)
        {
            scopes.Push(containingScope);
        }

        public bool Enter(FixedList<IStatementSyntax> statements, ITreeWalker walker)
        {
            var scopeDepth = scopes.Count;

            foreach (var statement in statements) walker.Walk(statement);

            // Remove any scopes created by variable declarations
            while (scopes.Count > scopeDepth) scopes.Pop();

            return false;
        }

        public void Exit(FixedList<IStatementSyntax> statements, ITreeWalker walker) { }

        public bool Enter(IStatementSyntax statement, ITreeWalker walker)
        {
            // Each variable declaration effectively starts a new scope after it, this
            // ensures a lookup returns the last declaration
            if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                scopes.Push(new NestedScope(ContainingScope, variableDeclaration.Yield(), Enumerable.Empty<ISymbol>()));

            return true;
        }

        public void Exit(IStatementSyntax statement, ITreeWalker walker)
        {
        }

        public override bool Enter(IExpressionSyntax expression, ITreeWalker walker)
        {
            switch (expression)
            {
                case IBlockExpressionSyntax _:
                    break;
                case INameExpressionSyntax nameExpression:
                    nameExpression.ContainingScope = ContainingScope;
                    break;
                case ITypeNameSyntax typeName:
                    typeName.ContainingScope = ContainingScope;
                    break;
                case IForeachExpressionSyntax foreachExpression:
                    walker.Walk(foreachExpression.TypeSyntax);
                    walker.Walk(foreachExpression.InExpression);
                    scopes.Push(new NestedScope(ContainingScope, foreachExpression.Yield(), Enumerable.Empty<ISymbol>()));
                    walker.Walk(foreachExpression.Block);
                    scopes.Pop();
                    return false;
            }

            return true;
        }

        public override bool Enter(ITransferSyntax transfer, ITreeWalker walker)
        {
            return true;
        }


        //public override void VisitBlockExpression(IBlockExpressionSyntax block, LexicalScope containingScope)
        //{
        //    if (block == null)
        //        return;

        //    var statements = block.Statements;
        //    VisitStatements(statements, containingScope);
        //}

        //public override void VisitNameExpression(
        //    INameExpressionSyntax nameExpression,
        //    LexicalScope containingScope)
        //{
        //    nameExpression.ContainingScope = containingScope;
        //}

        //public override void VisitForeachExpression(IForeachExpressionSyntax foreachExpression, LexicalScope containingScope)
        //{
        //    VisitType(foreachExpression.TypeSyntax, containingScope);
        //    VisitExpression(foreachExpression.InExpression, containingScope);
        //    containingScope = new NestedScope(containingScope, foreachExpression.Yield(), Enumerable.Empty<ISymbol>());
        //    VisitExpression(foreachExpression.Block, containingScope);
        //}

        //public override void VisitTypeName(ITypeNameSyntax typeName, LexicalScope containingScope)
        //{
        //    typeName.ContainingScope = containingScope;
        //}
    }
}
