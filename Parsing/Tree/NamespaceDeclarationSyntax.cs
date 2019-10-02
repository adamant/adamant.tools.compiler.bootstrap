using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NamespaceDeclarationSyntax : DeclarationSyntax, INamespaceDeclarationSyntax
    {
        /// <summary>
        /// Whether this namespace declaration is in the global namespace, the
        /// implicit file namespace is in the global namespace. As are namespaces
        /// declared using the package qualifier `namespace ::example { }`.
        /// </summary>
        public bool InGlobalNamespace { get; }
        public Name Name { get; }
        public Name FullName { get; }
        /// <summary>
        /// The name context is the part of the namespace determined by the file
        /// location and any containing namespace declarations.
        /// </summary>
        public RootName NameContext { get; }
        public FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        public FixedList<INonMemberDeclarationSyntax> Declarations { get; }

        public NamespaceDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            bool inGlobalNamespace,
            Name name,
            TextSpan nameSpan,
            RootName nameContext,
            FixedList<IUsingDirectiveSyntax> usingDirectives,
            FixedList<INonMemberDeclarationSyntax> declarations)
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
            return $"namespace {FullName} {{ … }}";
        }
    }
}
