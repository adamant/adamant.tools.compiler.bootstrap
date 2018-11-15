using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
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

        public override string ToString()
        {
            return Keyword.ToString();
        }
    }
}
