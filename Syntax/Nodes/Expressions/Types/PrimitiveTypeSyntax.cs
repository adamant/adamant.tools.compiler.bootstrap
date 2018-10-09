using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public class PrimitiveTypeSyntax : TypeSyntax
    {
        [NotNull]
        public KeywordToken Keyword { get; }

        public PrimitiveTypeSyntax([NotNull] KeywordToken keyword)
        {
            Requires.NotNull(nameof(keyword), keyword);
            Keyword = keyword;
        }
    }
}
