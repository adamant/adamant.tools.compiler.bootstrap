using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
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
        [NotNull] public Name Name { get; }
        [NotNull] public Name FullName { get; }
        [NotNull] public RootName NameContext { get; }
        [NotNull] public FixedList<UsingDirectiveSyntax> UsingDirectives { get; }
        [NotNull] public FixedList<DeclarationSyntax> Declarations { get; }

        public NamespaceDeclarationSyntax(
            bool inGlobalNamespace,
            [NotNull] Name name,
            TextSpan nameSpan,
            [NotNull] RootName nameContext,
            [NotNull] FixedList<UsingDirectiveSyntax> usingDirectives,
            [NotNull] FixedList<DeclarationSyntax> declarations)
            : base(nameSpan)
        {
            Name = name;
            FullName = nameContext.Qualify(name);
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            InGlobalNamespace = inGlobalNamespace;
            NameContext = nameContext;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
