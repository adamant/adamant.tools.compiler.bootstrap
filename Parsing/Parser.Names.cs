using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        private NameSyntax ParseName()
        {
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = identifier.Value;
            return new NameSyntax(identifier.Span, name);
        }
    }
}
