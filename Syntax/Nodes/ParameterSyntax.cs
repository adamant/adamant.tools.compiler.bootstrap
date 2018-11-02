using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public abstract class ParameterSyntax : SyntaxNode
    {
        public TextSpan Span { get; }

        protected ParameterSyntax(TextSpan span)
        {
            Span = span;
        }
    }
}
