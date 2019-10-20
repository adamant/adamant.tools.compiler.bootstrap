using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    [SuppressMessage("Performance", "CA1812:Class Never Instantiated")]
    internal class MutableExpressionSyntax : ExpressionSyntax, IMutableExpressionSyntax
    {
        public INameExpressionSyntax Expression { get; }

        public MutableExpressionSyntax(TextSpan span, INameExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"mut {Expression}";
        }
    }
}
