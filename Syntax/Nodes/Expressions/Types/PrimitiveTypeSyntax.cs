using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public class PrimitiveTypeSyntax : TypeSyntax
    {
        public SimpleToken Keyword { get; }

        public PrimitiveTypeSyntax(SimpleToken keyword)
        {
            Keyword = keyword;
        }
    }
}
