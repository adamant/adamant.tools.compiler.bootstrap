using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public FixedList<IUsingDirectiveSyntax> ParseUsingDirectives()
        {
            return AcceptMany(AcceptUsingDirective);
        }

        public IUsingDirectiveSyntax? AcceptUsingDirective()
        {
            var accept = Tokens.AcceptToken<IUsingKeywordToken>();
            if (accept == null)
                return null;
            var identifiers = AcceptOneOrMore<IIdentifierToken, IDotToken>(
                () => Tokens.AcceptToken<IIdentifierToken>());
            RootName name = GlobalNamespaceName.Instance;
            foreach (var identifier in identifiers)
                name = name.Qualify(identifier.Value);
            var semicolon = Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(accept.Span, semicolon);
            return new UsingDirectiveSyntax(span, (Name)name);
        }
    }
}
