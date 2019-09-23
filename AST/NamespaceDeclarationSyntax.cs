using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

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
        public Name Name { get; }
        public Name FullName { get; }
        public RootName NameContext { get; }
        public FixedList<UsingDirectiveSyntax> UsingDirectives { get; }
        public FixedList<DeclarationSyntax> Declarations { get; }

        public NamespaceDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            bool inGlobalNamespace,
            Name name,
            TextSpan nameSpan,
            RootName nameContext,
            FixedList<UsingDirectiveSyntax> usingDirectives,
            FixedList<DeclarationSyntax> declarations)
            : base(span, file, nameSpan)
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
