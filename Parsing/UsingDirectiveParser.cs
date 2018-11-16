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
        [NotNull] private readonly IListParser listParser;

        public UsingDirectiveParser(
            [NotNull] ITokenIterator tokens,
            [NotNull] INameParser qualifiedNameParser,
            [NotNull] IListParser listParser)
            : base(tokens)
        {
            this.qualifiedNameParser = qualifiedNameParser;
            this.listParser = listParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public FixedList<UsingDirectiveSyntax> ParseUsingDirectives()
        {
            return listParser.AcceptList(AcceptUsingDirective);
        }

        [CanBeNull]
        public UsingDirectiveSyntax AcceptUsingDirective()
        {
            if (!Tokens.Accept<IUsingKeywordToken>()) return null;
            var name = qualifiedNameParser.ParseName();
            Tokens.Expect<ISemicolonToken>();
            return new UsingDirectiveSyntax(name);
        }
    }
}
