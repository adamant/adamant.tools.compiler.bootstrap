using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public class AddStatement : SimpleStatement
    {
        [NotNull] public LValue LValue { get; }
        [NotNull] public LValue LeftOperand { get; }
        [NotNull] public LValue RightOperand { get; }

        public AddStatement([NotNull] LValue lValue, [NotNull] LValue leftOperand, [NotNull] LValue rightOperand)
        {
            Requires.NotNull(nameof(lValue), lValue);
            Requires.NotNull(nameof(leftOperand), leftOperand);
            Requires.NotNull(nameof(rightOperand), rightOperand);
            LValue = lValue;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }
    }
}
