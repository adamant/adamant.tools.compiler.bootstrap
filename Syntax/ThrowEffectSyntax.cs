using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ThrowEffectSyntax : EffectSyntax
    {
        [NotNull] public IThrowKeywordToken ThrowKeyword { get; }
        [NotNull] public SeparatedListSyntax<ThrowEffectEntrySyntax> Exceptions { get; }

        public ThrowEffectSyntax(
            [NotNull] IThrowKeywordToken throwKeyword,
            [NotNull] SeparatedListSyntax<ThrowEffectEntrySyntax> exceptions)
        {
            Requires.NotNull(nameof(throwKeyword), throwKeyword);
            Requires.NotNull(nameof(exceptions), exceptions);
            ThrowKeyword = throwKeyword;
            Exceptions = exceptions;
        }
    }
}
