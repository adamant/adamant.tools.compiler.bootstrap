using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NamedParameterSyntax : ParameterSyntax, INamedParameterSyntax
    {
        public TypeSyntax TypeSyntax { get; }
        public ExpressionSyntax DefaultValue { get; }

        public NamedParameterSyntax(
            TextSpan span,
            bool isMutableBinding,
            Name fullName,
            TypeSyntax typeSyntax,
            ExpressionSyntax defaultValue)
            : base(span, isMutableBinding, fullName)
        {
            TypeSyntax = typeSyntax;
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            var defaultValue = DefaultValue != null ? " = " + DefaultValue : "";
            return $"{Name}: {TypeSyntax}{defaultValue}";
        }
    }
}
