using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public class PrimitiveTypeSyntax : TypeSyntax
    {
        public Token Keyword { get; }

        public PrimitiveTypeSyntax(Token keyword)
            : base(keyword.Yield())
        {
            Keyword = keyword;
        }
    }
}
