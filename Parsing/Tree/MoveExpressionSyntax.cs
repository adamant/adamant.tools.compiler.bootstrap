using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    [SuppressMessage("Performance", "CA1812:Class Never Instantiated")]
    internal class MoveExpressionSyntax : ExpressionSyntax, IMoveExpressionSyntax
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier",
            Justification = "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;
        public ref IExpressionSyntax Referent => ref referent;


        public MoveExpressionSyntax(TextSpan span, INameExpressionSyntax referent)
            : base(span)
        {
            this.referent = referent;
        }

        public override string ToString()
        {
            return $"move {Referent}";
        }
    }
}
