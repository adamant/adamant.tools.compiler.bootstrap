using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives
{
    public class CompilationUnitNamespaceSyntax : SyntaxNode
    {
        [CanBeNull]
        public NamespaceKeywordToken NamespaceKeyword { get; }
        [NotNull]
        public NameSyntax Name { get; }
        [CanBeNull]
        public SemicolonToken Semicolon { get; }

        public CompilationUnitNamespaceSyntax(
            [CanBeNull] NamespaceKeywordToken namespaceKeyword,
            [NotNull] NameSyntax name,
            [CanBeNull] SemicolonToken semicolon)
        {
            Requires.NotNull(nameof(name), name);
            NamespaceKeyword = namespaceKeyword;
            Name = name;
            Semicolon = semicolon;
        }
    }
}
