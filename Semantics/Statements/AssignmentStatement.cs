using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public class AssignmentStatement : SimpleStatement
    {
        [NotNull] public LValue LValue { get; }
        [NotNull] public RValue RValue { get; }

        public AssignmentStatement([NotNull] LValue lValue, [NotNull] RValue rValue)
        {
            Requires.NotNull(nameof(lValue), lValue);
            Requires.NotNull(nameof(rValue), rValue);
            LValue = lValue;
            RValue = rValue;
        }
    }
}
