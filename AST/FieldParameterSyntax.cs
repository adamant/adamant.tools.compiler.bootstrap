using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class FieldParameterSyntax : ParameterSyntax
    {
        [CanBeNull] public ExpressionSyntax DefaultValue { get; }

        public FieldParameterSyntax(
            TextSpan span,
            [NotNull] Name fullName,
            [CanBeNull] ExpressionSyntax defaultValue)
            : base(span, false, fullName)
        {
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
