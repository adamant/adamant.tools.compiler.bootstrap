using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class UsingDirectiveParser : Parser, IUsingDirectiveParser
    {
        [NotNull] private readonly INameParser qualifiedNameParser;

        public UsingDirectiveParser(
            [NotNull] ITokenIterator tokens,
            [NotNull] INameParser qualifiedNameParser)
            : base(tokens)
        {
            this.qualifiedNameParser = qualifiedNameParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public FixedList<UsingDirectiveSyntax> ParseUsingDirectives()
        {
            // TODO use list parser instead
            var directives = new List<UsingDirectiveSyntax>();
            while (Tokens.Current is IUsingKeywordToken)
                directives.Add(ParseUsingDirective());

            return directives.ToFixedList();
        }

        [NotNull]
        public UsingDirectiveSyntax ParseUsingDirective()
        {
            Tokens.Expect<IUsingKeywordToken>();
            var name = qualifiedNameParser.ParseName();
            Tokens.Expect<ISemicolonToken>();
            return new UsingDirectiveSyntax(name);
        }
    }
}
