using System;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    public class Restriction : IEquatable<Restriction>
    {
        public int Place { get; }

        // Whether one can move the value out of this place
        public bool CanTake { get; }

        // Whether one can mutate the data in this place
        public bool CanMutate { get; }

        // Whether one can borrow the data in this place mutably
        public bool CanBorrowMutable { get; }

        // Whether one can borrow the data in this place immutably
        public bool CanBorrowImmutable { get; }

        public Restriction(int place, bool mutableBorrow)
        {
            Place = place;
            // Any restriction implies you can't take
            CanTake = false;
            CanMutate = false; // When would this be true?
            CanBorrowMutable = false; // When would this be true?
            CanBorrowImmutable = !mutableBorrow;
        }

        public override string ToString()
        {
            return $"%{Place}...";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Restriction);
        }

        public bool Equals(Restriction other)
        {
            return other != null &&
                   Place == other.Place &&
                   CanTake == other.CanTake &&
                   CanMutate == other.CanMutate &&
                   CanBorrowMutable == other.CanBorrowMutable &&
                   CanBorrowImmutable == other.CanBorrowImmutable;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Place, CanTake, CanMutate, CanBorrowMutable, CanBorrowImmutable);
        }

        public static bool operator ==(Restriction restriction1, Restriction restriction2)
        {
            return EqualityComparer<Restriction>.Default.Equals(restriction1, restriction2);
        }

        public static bool operator !=(Restriction restriction1, Restriction restriction2)
        {
            return !(restriction1 == restriction2);
        }
    }
}
