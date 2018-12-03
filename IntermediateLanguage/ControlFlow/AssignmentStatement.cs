using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class AssignmentStatement : ExpressionStatement
    {
        [NotNull] public Place Place { get; }
        [NotNull] public Value Value { get; }

        public AssignmentStatement(
            int blockNumber,
            int number,
            [NotNull] Place place,
            [NotNull] Value value)
            : base(blockNumber, number)
        {
            Requires.NotNull(nameof(place), place);
            Requires.NotNull(nameof(value), value);
            Place = place;
            Value = value;
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"{Place} = {Value};";
        }
    }
}
