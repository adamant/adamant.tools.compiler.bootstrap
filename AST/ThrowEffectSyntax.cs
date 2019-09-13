using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class ThrowEffectSyntax : EffectSyntax
    {
        public FixedList<ThrowEffectEntrySyntax> Exceptions { get; }

        public ThrowEffectSyntax(
            FixedList<ThrowEffectEntrySyntax> exceptions)
        {
            Exceptions = exceptions;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
