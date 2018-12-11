using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    /// <summary>
    /// A loaning out of a reference into a variable
    /// </summary>
    public class Loan : Claim
    {
        public FixedList<Restriction> Restrictions { get; }

        /// <param name="variable">Variable the reference is loaned to</param>
        /// <param name="operand">The operand loaned from</param>
        /// <param name="objectId">Which object is loaned</param>
        public Loan(int variable, Operand operand, int objectId)
            : base(variable, objectId)
        {
            var restrictions = new List<Restriction>();
            GatherRestrictions(operand, restrictions);
            Restrictions = restrictions.ToFixedList();
        }

        public Loan(int variable, int objectId)
            : base(variable, objectId)
        {
            Restrictions = Enumerable.Empty<Restriction>().ToFixedList();
        }

        private static void GatherRestrictions(Operand operand, List<Restriction> restrictions)
        {
            switch (operand)
            {
                case Place place:
                    //case VariableReference variable:
                    restrictions.Add(new Restriction(place.CoreVariable(), false));
                    break;
                default:
                    throw NonExhaustiveMatchException.For(operand);
            }
        }
    }
}
