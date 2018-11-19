using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NamespaceDeclarationSyntax : DeclarationSyntax
    {
        /// <summary>
        /// Whether this namespace declaration is in the global namespace, the
        /// implicit file namespace is in the global namespace. As are namespaces
        /// declared using the package qualifier `namespace ::example { }`.
        /// </summary>
        public bool InGlobalNamespace { get; }
        [NotNull] public FixedList<string> Name { get; }

        /// <summary>
        /// The implicit file namespace doesn't have a span.
        /// </summary>
        public TextSpan? Span { get; }
        [NotNull] public FixedList<UsingDirectiveSyntax> UsingDirectives { get; }
        [NotNull] public FixedList<DeclarationSyntax> Declarations { get; }

        public NamespaceDeclarationSyntax(
            bool inGlobalNamespace,
            [NotNull] FixedList<string> name,
            TextSpan? span,
            [NotNull] FixedList<UsingDirectiveSyntax> usingDirectives,
            [NotNull] FixedList<DeclarationSyntax> declarations)
        {
            Name = name;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            InGlobalNamespace = inGlobalNamespace;
            Span = span;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
