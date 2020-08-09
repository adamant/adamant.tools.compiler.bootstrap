using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NamespaceDeclarationSyntax : DeclarationSyntax, INamespaceDeclarationSyntax
    {
        private LexicalScope<IPromise<Symbol>>? containingLexicalScope;
        public LexicalScope<IPromise<Symbol>> ContainingLexicalScope
        {
            get =>
                containingLexicalScope
                ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
            set
            {
                if (containingLexicalScope != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
                containingLexicalScope = value;
            }
        }

        public NamespaceName ContainingNamespaceName { get; }

        /// <summary>
        /// Whether this namespace declaration is in the global namespace, the
        /// implicit file namespace is in the global namespace. As are namespaces
        /// declared using the package qualifier `namespace ::example { }`.
        /// </summary>
        public bool IsGlobalQualified { get; }
        public NamespaceName DeclaredNames { get; }
        public NamespaceName FullName { get; }
        public Promise<NamespaceOrPackageSymbol> Symbol { get; } = new Promise<NamespaceOrPackageSymbol>();
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
            : base(span, file, nameSpan)
        {
            ContainingNamespaceName = containingNamespaceName;
            DeclaredNames = declaredNames;
            FullName = containingNamespaceName.Qualify(declaredNames);
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            IsGlobalQualified = isGlobalQualified;
        }

        public override string ToString()
        {
            return $"namespace ::{FullName} {{ … }}";
        }
    }
}
