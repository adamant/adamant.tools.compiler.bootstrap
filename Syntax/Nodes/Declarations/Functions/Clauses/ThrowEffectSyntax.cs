using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Clauses
{
    public class ThrowEffectSyntax : EffectSyntax
    {
        [NotNull] public ThrowKeywordToken ThrowKeyword { get; }
        [NotNull] public SeparatedListSyntax<ThrowEffectEntrySyntax> Exceptions { get; }

        public ThrowEffectSyntax(
            [NotNull] ThrowKeywordToken throwKeyword,
            [NotNull] SeparatedListSyntax<ThrowEffectEntrySyntax> exceptions)
        {
            Requires.NotNull(nameof(throwKeyword), throwKeyword);
            Requires.NotNull(nameof(exceptions), exceptions);
            ThrowKeyword = throwKeyword;
            Exceptions = exceptions;
        }
    }
}
