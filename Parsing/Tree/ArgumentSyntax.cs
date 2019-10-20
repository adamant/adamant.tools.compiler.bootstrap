using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ArgumentSyntax : Syntax, IArgumentSyntax
    {
        private IExpressionSyntax expression;
        public ref IExpressionSyntax Expression => ref expression;

        public ArgumentSyntax(IExpressionSyntax expression)
            : base(expression.Span)
        {
            this.expression = expression;
        }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }
}
