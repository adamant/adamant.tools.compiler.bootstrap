using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class AssignmentStatement : SimpleStatement
    {
        [NotNull] public Place Place { get; }
        [NotNull] public RValue RValue { get; }

        public AssignmentStatement([NotNull] Place place, [NotNull] RValue rValue)
        {
            Requires.NotNull(nameof(place), place);
            Requires.NotNull(nameof(rValue), rValue);
            Place = place;
            RValue = rValue;
        }
    }
}
