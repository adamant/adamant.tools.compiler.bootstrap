using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class UsingDirectiveParser : IParser<UsingDirectiveSyntax>
    {
        [NotNull]
        private readonly IParser<NameSyntax> qualifiedNameParser;

        public UsingDirectiveParser([NotNull] IParser<NameSyntax> qualifiedNameParser)
        {
            this.qualifiedNameParser = qualifiedNameParser;
        }

        [NotNull]
        public UsingDirectiveSyntax Parse([NotNull] ITokenStream tokens, IDiagnosticsCollector diagnostics)
        {
            var usingKeyword = tokens.Expect<IUsingKeywordToken>();
            var name = qualifiedNameParser.Parse(tokens, diagnostics);
            var semicolon = tokens.Expect<ISemicolonToken>();
            return new UsingDirectiveSyntax(usingKeyword, name, semicolon);
        }
    }
}
