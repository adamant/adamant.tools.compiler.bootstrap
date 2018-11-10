using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ThrowEffectSyntax : EffectSyntax
    {
        [NotNull] public FixedList<ThrowEffectEntrySyntax> Exceptions { get; }

        public ThrowEffectSyntax(
            [NotNull] FixedList<ThrowEffectEntrySyntax> exceptions)
        {
            Requires.NotNull(nameof(exceptions), exceptions);
            Exceptions = exceptions;
        }
    }
}
