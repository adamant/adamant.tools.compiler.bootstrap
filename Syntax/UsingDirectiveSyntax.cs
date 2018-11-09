using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class UsingDirectiveSyntax : NonTerminal
    {
        [NotNull] public IUsingKeywordTokenPlace UsingKeyword { get; }
        [NotNull] public NameSyntax Name { get; }
        [NotNull] public ISemicolonTokenPlace Semicolon { get; }

        public UsingDirectiveSyntax(
            [NotNull] IUsingKeywordTokenPlace usingKeyword,
            [NotNull] NameSyntax name,
            [NotNull] ISemicolonTokenPlace semicolon)
        {
            Requires.NotNull(nameof(name), name);
            UsingKeyword = usingKeyword;
            Name = name;
            Semicolon = semicolon;
        }
    }
}
