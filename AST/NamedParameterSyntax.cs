using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NamedParameterSyntax : ParameterSyntax
    {
        public bool IsParams { get; }
        public bool MutableBinding { get; }
        [NotNull] public SimpleName Name { get; }
        [NotNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public ExpressionSyntax DefaultValue { get; }

        public NamedParameterSyntax(
            TextSpan span,
            bool isParams,
            bool mutableBinding,
            [NotNull] string name,
            [NotNull] ExpressionSyntax typeExpression,
            [CanBeNull] ExpressionSyntax defaultValue)
            : base(span)
        {
            IsParams = isParams;
            MutableBinding = mutableBinding;
            Name = new SimpleName(name);
            TypeExpression = typeExpression;
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
