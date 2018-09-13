using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.RValues;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BorrowChecker
{
    public class Loan : Claim
    {
        public IReadOnlyList<Restriction> Restrictions { get; }

        public Loan(int variable, RValue rvalue, int @object)
            : base(variable, @object)
        {
        }
    }
}
