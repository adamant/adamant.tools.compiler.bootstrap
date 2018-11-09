using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class AssignmentStatement : Statement
    {
        [NotNull] public Place Place { get; }
        [NotNull] public IValue Value { get; }

        public AssignmentStatement([NotNull] Place place, [NotNull] IValue value)
        {
            Requires.NotNull(nameof(place), place);
            Requires.NotNull(nameof(value), value);
            Place = place;
            Value = value;
        }
    }
}
