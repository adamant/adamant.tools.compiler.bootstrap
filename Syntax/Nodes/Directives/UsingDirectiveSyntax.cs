using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives
{
    public class UsingDirectiveSyntax : SyntaxNode
    {
        [CanBeNull]
        public UsingKeywordToken UsingKeyword { get; }

        [NotNull]
        public NameSyntax Name { get; }

        [CanBeNull]
        public SemicolonToken Semicolon { get; }

        public UsingDirectiveSyntax(
            [CanBeNull] UsingKeywordToken usingKeyword,
            [NotNull] NameSyntax name,
            [CanBeNull] SemicolonToken semicolon)
        {
            Requires.NotNull(nameof(name), name);
            UsingKeyword = usingKeyword;
            Name = name;
            Semicolon = semicolon;
        }
    }
}
