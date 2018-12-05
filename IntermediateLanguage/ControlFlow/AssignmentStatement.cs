namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class AssignmentStatement : ExpressionStatement
    {
        public Place Place { get; }
        public Value Value { get; }

        public AssignmentStatement(
            int blockNumber,
            int number,
            Place place,
            Value value)
            : base(blockNumber, number)
        {
            Place = place;
            Value = value;
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"{Place} = {Value}";
        }
    }
}