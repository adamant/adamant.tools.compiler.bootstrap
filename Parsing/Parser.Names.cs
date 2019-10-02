using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        private INameExpressionSyntax ParseNameExpression()
        {
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = new SimpleName(identifier.Value);
            return new NameExpressionSyntax(identifier.Span, name);
        }
    }
}
