using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class ParameterSyntax : Syntax
    {
        public TextSpan Span { get; }

        protected ParameterSyntax(TextSpan span)
        {
            Span = span;
        }
    }
}
