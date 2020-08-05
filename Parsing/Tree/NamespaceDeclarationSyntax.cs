using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
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
        public bool IsGlobalQualified { get; }

        public NamespaceName DeclaredNames { get; }

        /// <summary>
        /// The name context is the part of the namespace determined by the file
        /// location and any containing namespace declarations.
        /// </summary>
        public NamespaceName NameContext { get; }

        public NamespaceName Name { get; }

        public FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        public FixedList<INonMemberDeclarationSyntax> Declarations { get; }

        public NamespaceDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            bool isGlobalQualified,
            NamespaceName declaredNames,
            TextSpan nameSpan,
            NamespaceName nameContext,
            FixedList<IUsingDirectiveSyntax> usingDirectives,
            FixedList<INonMemberDeclarationSyntax> declarations)
            : base(span, file, nameSpan)
        {
            DeclaredNames = declaredNames;
            NameContext = nameContext;
            Name = nameContext.Qualify(declaredNames);
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            IsGlobalQualified = isGlobalQualified;
        }

        public override string ToString()
        {
            return $"namespace ::{Name} {{ … }}";
        }
    }
}
