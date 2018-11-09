using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SelfParameterSyntax : ParameterSyntax
    {
        [CanBeNull] public MutableKeywordToken MutableKeyword { get; }
        [NotNull] public SelfKeywordToken SelfKeyword { get; }

        public SelfParameterSyntax(
            [CanBeNull] MutableKeywordToken mutableKeyword,
            [NotNull] SelfKeywordToken selfKeyword)
            : base(TextSpan.Covering(mutableKeyword?.Span, selfKeyword.Span))
        {
            Requires.NotNull(nameof(selfKeyword), selfKeyword);
            SelfKeyword = selfKeyword;
            MutableKeyword = mutableKeyword;
        }
    }
}
