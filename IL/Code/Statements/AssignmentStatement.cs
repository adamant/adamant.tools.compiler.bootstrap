using Adamant.Tools.Compiler.Bootstrap.IL.Code.LValues;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.RValues;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements
{
    public class AssignmentStatement : Statement
    {
        public LValue LValue { get; }
        public RValue RValue { get; }

        public AssignmentStatement(LValue lValue, RValue rValue)
        {
            LValue = lValue;
            RValue = rValue;
        }

        internal override void ToString(AsmBuilder builder)
        {
            builder.AppendLine($"{LValue} = borrow {RValue}");
        }
    }
}
