using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ArgumentSyntax : Syntax, IArgumentSyntax
    {
        private ExpressionSyntax value;

        public ExpressionSyntax Value => value;
        public ref ExpressionSyntax ValueRef => ref value;

        public ArgumentSyntax(
            TextSpan span,
            ExpressionSyntax value)
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
