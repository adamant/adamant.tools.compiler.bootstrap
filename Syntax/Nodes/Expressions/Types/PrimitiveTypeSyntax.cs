using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public class PrimitiveTypeSyntax : TypeSyntax
    {
        [NotNull] public IPrimitiveTypeToken Keyword { get; }

        public PrimitiveTypeSyntax([NotNull] IPrimitiveTypeToken keyword)
            : base(keyword.Span)
        {
            Requires.NotNull(nameof(keyword), keyword);
            Keyword = keyword;
        }
    }
}
