using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NamedParameterSyntax : ParameterSyntax, INamedParameterSyntax
    {
        public override bool IsMutableBinding { get; }
        public ITypeSyntax TypeSyntax { get; }
        public IExpressionSyntax? DefaultValue { get; }

        public NamedParameterSyntax(
            TextSpan span,
            bool isMutableBinding,
            Name fullName,
            ITypeSyntax typeSyntax,
            IExpressionSyntax? defaultValue)
            : base(span, fullName)
        {
            IsMutableBinding = isMutableBinding;
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
