using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class FieldParameterSyntax : ParameterSyntax
    {
        [NotNull] public SimpleName Name { get; }
        [CanBeNull] public ExpressionSyntax DefaultValue { get; }

        public FieldParameterSyntax(
            TextSpan span,
            [NotNull] string value,
            [CanBeNull] ExpressionSyntax defaultValue)
            : base(span)
        {
            Name = new SimpleName(value);
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
