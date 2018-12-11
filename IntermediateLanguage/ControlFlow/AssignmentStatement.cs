namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class AssignmentStatement : ExpressionStatement
    {
        public Place Place { get; }
        public Value Value { get; }

        public AssignmentStatement(Place place, Value value)
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
