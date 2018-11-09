using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class IntegerLiteralStatement : SimpleStatement
    {
        [NotNull] public Place Place { get; }
        public BigInteger Value { get; }


        public IntegerLiteralStatement(
            [NotNull] Place place,
             BigInteger value)
        {
            Requires.NotNull(nameof(place), place);
            Place = place;
            Value = value;
        }
    }
}
