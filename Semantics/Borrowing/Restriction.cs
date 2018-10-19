namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    public class Restriction
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
    }
}
