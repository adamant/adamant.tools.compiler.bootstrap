using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ThrowEffectSyntax : EffectSyntax
    {
        [NotNull] public FixedList<ThrowEffectEntrySyntax> Exceptions { get; }

        public ThrowEffectSyntax(
            [NotNull] FixedList<ThrowEffectEntrySyntax> exceptions)
        {
            Exceptions = exceptions;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
