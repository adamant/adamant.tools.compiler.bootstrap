using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types
{
    public class PrimitiveTypeSyntax : TypeSyntax
    {
        public Token Keyword => Children.Cast<Token>().Single();

        public PrimitiveTypeSyntax(Token keyword)
            : base(keyword.Yield())
        {
        }
    }
}
