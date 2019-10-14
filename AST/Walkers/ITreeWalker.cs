using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface ITreeWalker
    {
        void Walk(IDeclarationSyntax? declaration);
        void Walk(ITypeSyntax? type);
        void Walk(FixedList<IStatementSyntax>? statements);
        void Walk(IStatementSyntax? statement);
        void Walk(IBlockOrResultSyntax? blockOrResult);
        void Walk(IElseClauseSyntax? elseClause);
        void Walk(IBlockExpressionSyntax? blockExpression);
        void Walk(IExpressionSyntax? expression);
    }
}
