using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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
        public FixedList<UsingDirectiveSyntax> ParseUsingDirectives(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var directives = new List<UsingDirectiveSyntax>();
            while (tokens.Current is IUsingKeywordToken)
                directives.Add(ParseUsingDirective(tokens, diagnostics));

            return directives.ToFixedList();
        }

        [NotNull]
        public UsingDirectiveSyntax ParseUsingDirective(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var usingKeyword = tokens.Consume<IUsingKeywordTokenPlace>();
            var name = qualifiedNameParser.ParseName(tokens, diagnostics);
            var semicolon = tokens.Consume<ISemicolonTokenPlace>();
            return new UsingDirectiveSyntax(usingKeyword, name, semicolon);
        }
    }
}
