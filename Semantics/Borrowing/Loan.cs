using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    /// <summary>
    /// A loaning out of a reference into a variable
    /// </summary>
    public class Loan : Claim
    {
        [NotNull] public IReadOnlyList<Restriction> Restrictions { get; }

        /// <param name="variable">Variable the reference is loaned to</param>
        /// <param name="rvalue">The value loaned from</param>
        /// <param name="object">Which object is loaned</param>
        public Loan(int variable, [NotNull] RValue rvalue, int @object)
            : base(variable, @object)
        {
            Requires.NotNull(nameof(rvalue), rvalue);
            var restrictions = new List<Restriction>();
            GatherRestrictions(rvalue, restrictions);
            Restrictions = restrictions.AsReadOnly().AssertNotNull();
        }

        public Loan(int variable, int @object)
            : base(variable, @object)
        {
            Restrictions = Enumerable.Empty<Restriction>().ToReadOnlyList();
        }

        private static void GatherRestrictions([NotNull] RValue rvalue, [NotNull] List<Restriction> restrictions)
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
