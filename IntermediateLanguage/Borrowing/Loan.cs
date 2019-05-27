using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// A loaning out of a reference into a variable
    /// </summary>
    public class Loan : Claim, IEquatable<Loan>
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

        public override string ToString()
        {
            return $"Loan of #{ObjectId} to %{Variable} ({string.Join(",", Restrictions)})";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Loan);
        }

        public override bool Equals(Claim other)
        {
            return Equals(other as Loan);
        }

        public bool Equals(Loan other)
        {
            return other != null &&
                   base.Equals(other) &&
                   Restrictions.SequenceEqual(other.Restrictions);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(base.GetHashCode());
            foreach (var restriction in Restrictions) hash.Add(restriction);
            return hash.ToHashCode();
        }

        public static bool operator ==(Loan loan1, Loan loan2)
        {
            return EqualityComparer<Loan>.Default.Equals(loan1, loan2);
        }

        public static bool operator !=(Loan loan1, Loan loan2)
        {
            return !(loan1 == loan2);
        }
    }
}
