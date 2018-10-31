using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class UsingDirectiveParser : IUsingDirectiveParser
    {
        [NotNull] private readonly INameParser qualifiedNameParser;

        public UsingDirectiveParser([NotNull] INameParser qualifiedNameParser)
        {
            this.qualifiedNameParser = qualifiedNameParser;
        }

        [NotNull]
        public UsingDirectiveSyntax ParseUsingDirective(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var usingKeyword = tokens.Expect<IUsingKeywordToken>();
            var name = qualifiedNameParser.ParseName(tokens, diagnostics);
            var semicolon = tokens.Expect<ISemicolonToken>();
            return new UsingDirectiveSyntax(usingKeyword, name, semicolon);
        }
    }
}
