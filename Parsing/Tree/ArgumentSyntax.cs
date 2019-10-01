using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ArgumentSyntax : Syntax, IArgumentSyntax
    {
        private IExpressionSyntax value;

        public IExpressionSyntax Value => value;
        public ref IExpressionSyntax ValueRef => ref value;

        public ArgumentSyntax(
            TextSpan span,
            IExpressionSyntax value)
            : base(span)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }
    }
}
