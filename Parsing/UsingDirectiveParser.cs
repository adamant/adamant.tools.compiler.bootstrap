using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class UsingDirectiveParser : IUsingDirectiveParser
    {
        [NotNull] private readonly INameParser qualifiedNameParser;

        public UsingDirectiveParser([NotNull] INameParser qualifiedNameParser)
        {
            this.qualifiedNameParser = qualifiedNameParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public SyntaxList<UsingDirectiveSyntax> ParseUsingDirectives(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var directives = new List<UsingDirectiveSyntax>();
            while (tokens.Current is UsingKeywordToken)
                directives.Add(ParseUsingDirective(tokens, diagnostics));

            return directives.ToSyntaxList();
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
