using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.LValues;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public class AddStatement : SimpleStatement
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
    }
}
