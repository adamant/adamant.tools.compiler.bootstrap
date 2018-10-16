using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.LValues;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.RValues;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public class AssignmentStatement : SimpleStatement
    {
        public LValue LValue { get; }
        public RValue RValue { get; }

        public AssignmentStatement(LValue lValue, RValue rValue)
        {
            LValue = lValue;
            RValue = rValue;
        }
    }
}
