using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ArgumentSyntax : Syntax, IArgumentSyntax
    {
        private IExpressionSyntax value;
        public ref IExpressionSyntax Value => ref value;

        public ArgumentSyntax(
            TextSpan span,
            IExpressionSyntax value)
            : base(span)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
