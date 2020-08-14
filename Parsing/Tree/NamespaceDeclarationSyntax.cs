using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NamespaceDeclarationSyntax : DeclarationSyntax, INamespaceDeclarationSyntax
    {
        public NamespaceName ContainingNamespaceName { get; }

        private NamespaceOrPackageSymbol? containingNamespaceSymbol;
        public NamespaceOrPackageSymbol ContainingNamespaceSymbol
        {
            get =>
                containingNamespaceSymbol
                ?? throw new InvalidOperationException($"{ContainingNamespaceSymbol} not yet assigned");
            set
            {
                if (containingNamespaceSymbol != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingNamespaceSymbol)} repeatedly");
                containingNamespaceSymbol = value;
            }
        }

        /// <summary>
        /// Whether this namespace declaration is in the global namespace, the
        /// implicit file namespace is in the global namespace. As are namespaces
        /// declared using the package qualifier `namespace ::example { }`.
        /// </summary>
        public bool IsGlobalQualified { get; }
        public NamespaceName DeclaredNames { get; }
        public new Name Name { get; }
        public NamespaceName FullName { get; }
        public new Promise<NamespaceOrPackageSymbol> Symbol { get; }
        public FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        public FixedList<INonMemberDeclarationSyntax> Declarations { get; }

        public NamespaceDeclarationSyntax(
            NamespaceName containingNamespaceName,
            TextSpan span,
            CodeFile file,
            bool isGlobalQualified,
            NamespaceName declaredNames,
            TextSpan nameSpan,
            FixedList<IUsingDirectiveSyntax> usingDirectives,
            FixedList<INonMemberDeclarationSyntax> declarations)
            : base(span, file, declaredNames.Segments[^1], nameSpan, new Promise<NamespaceOrPackageSymbol>())
        {
            ContainingNamespaceName = containingNamespaceName;
            DeclaredNames = declaredNames;
            FullName = containingNamespaceName.Qualify(declaredNames);
            Name = declaredNames.Segments[^1];
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            IsGlobalQualified = isGlobalQualified;
            Symbol = (Promise<NamespaceOrPackageSymbol>)base.Symbol;
        }

        public override string ToString()
        {
            return $"namespace ::{FullName} {{ … }}";
        }
    }
}
