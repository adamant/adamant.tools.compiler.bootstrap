using Adamant.Tools.Compiler.Bootstrap.IL.Code.LValues;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements
{
    public class AddStatement : Statement
    {
        public LValue LValue { get; }
        public LValue LeftOperand { get; }
        public LValue RightOperand { get; }

        public AddStatement(LValue lValue, LValue leftOperand, LValue rightOperand)
        {
            LValue = lValue;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        internal override void ToString(AsmBuilder builder)
        {
            builder.AppendLine($"{LValue} = {LeftOperand} + {RightOperand}");
        }
    }
}
