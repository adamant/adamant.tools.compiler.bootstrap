using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class UsingDirectiveSyntax : SyntaxNode
    {
        [NotNull] public IUsingKeywordToken UsingKeyword { get; }
        [NotNull] public NameSyntax Name { get; }
        [NotNull] public ISemicolonToken Semicolon { get; }

        public UsingDirectiveSyntax(
            [NotNull] IUsingKeywordToken usingKeyword,
            [NotNull] NameSyntax name,
            [NotNull] ISemicolonToken semicolon)
        {
            Requires.NotNull(nameof(name), name);
            UsingKeyword = usingKeyword;
            Name = name;
            Semicolon = semicolon;
        }
    }
}
