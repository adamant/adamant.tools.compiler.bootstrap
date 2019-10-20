using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    [SuppressMessage("Performance", "CA1812:Class Never Instantiated")]
    internal class MoveExpressionSyntax : ExpressionSyntax, IMoveExpressionSyntax
    {
        public INameExpressionSyntax Referent { get; }

        public MoveExpressionSyntax(TextSpan span, INameExpressionSyntax referent)
            : base(span)
        {
            Referent = referent;
        }

        public override string ToString()
        {
            return $"move {Referent}";
        }
    }
}
