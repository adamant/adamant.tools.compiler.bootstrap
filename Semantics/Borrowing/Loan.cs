using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.LValues;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.RValues;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.BorrowChecker
{
    public class Loan : Claim
    {
        public IReadOnlyList<Restriction> Restrictions { get; }

        public Loan(int variable, RValue rvalue, int @object)
            : base(variable, @object)
        {
            var restrictions = new List<Restriction>();
            GatherRestrictions(rvalue, restrictions);
            Restrictions = restrictions.AsReadOnly();
        }

        private static void GatherRestrictions(RValue rvalue, List<Restriction> restrictions)
        {
            switch (rvalue)
            {
                case VariableReference variable:
                    restrictions.Add(new Restriction(variable.VariableNumber, false));
                    break;
                default:
                    throw NonExhaustiveMatchException.For(rvalue);
            }
        }
    }
}
