using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives
{
    public class CompilationUnitNamespaceSyntax : SyntaxNode
    {
        [NotNull] public INamespaceKeywordToken NamespaceKeyword { get; }
        [NotNull] public NameSyntax Name { get; }
        [NotNull] public ISemicolonToken Semicolon { get; }

        public CompilationUnitNamespaceSyntax(
            [NotNull] INamespaceKeywordToken namespaceKeyword,
            [NotNull] NameSyntax name,
            [NotNull] ISemicolonToken semicolon)
        {
            Requires.NotNull(nameof(name), name);
            NamespaceKeyword = namespaceKeyword;
            Name = name;
            Semicolon = semicolon;
        }
    }
}
