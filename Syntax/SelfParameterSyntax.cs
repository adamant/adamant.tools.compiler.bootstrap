using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SelfParameterSyntax : ParameterSyntax
    {
        [CanBeNull] public IMutableKeywordToken MutableKeyword { get; }
        [NotNull] public ISelfKeywordTokenPlace SelfKeyword { get; }

        public SelfParameterSyntax(
            [CanBeNull] IMutableKeywordToken mutableKeyword,
            [NotNull] ISelfKeywordTokenPlace selfKeyword)
            : base(TextSpan.Covering(mutableKeyword?.Span, selfKeyword.Span))
        {
            Requires.NotNull(nameof(selfKeyword), selfKeyword);
            SelfKeyword = selfKeyword;
            MutableKeyword = mutableKeyword;
        }
    }
}
