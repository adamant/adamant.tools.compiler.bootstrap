using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
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
        public Loan(int variable, [NotNull] IValue rvalue, int @object)
            : base(variable, @object)
        {
            Requires.NotNull(nameof(rvalue), rvalue);
            var restrictions = new List<Restriction>();
            GatherRestrictions(rvalue, restrictions);
            Restrictions = restrictions.AsReadOnly().NotNull();
        }

        public Loan(int variable, int @object)
            : base(variable, @object)
        {
            Restrictions = Enumerable.Empty<Restriction>().ToReadOnlyList();
        }

        private static void GatherRestrictions([NotNull] IValue rvalue, [NotNull] List<Restriction> restrictions)
        {
            switch (rvalue)
            {
                case CopyPlace copyPlace:
                    //case VariableReference variable:
                    restrictions.Add(new Restriction(copyPlace.Place.CoreVariable(), false));
                    break;
                default:
                    throw NonExhaustiveMatchException.For(rvalue);
            }
        }
    }
}
