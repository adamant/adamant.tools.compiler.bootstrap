using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations
{
    public class NamespaceBuilder
    {
        [NotNull] private readonly NameBuilder nameBuilder;

        public NamespaceBuilder([NotNull] NameBuilder nameBuilder)
        {
            this.nameBuilder = nameBuilder;
        }

        [NotNull]
        [ItemNotNull]
        public IReadOnlyList<Namespace> GatherNamespaces([NotNull] PackageSyntax packageSyntax)
        {
            var namespaces = new HashSet<Name>();
            foreach (var compilationUnit in packageSyntax.CompilationUnits)
                GatherNamespaces(GlobalNamespaceName.Instance, compilationUnit.Namespace, namespaces);

            return namespaces.Select(ns => new Namespace(ns)).ToReadOnlyList();
        }

        private void GatherNamespaces(
            [NotNull] RootName containingName,
            [NotNull] DeclarationSyntax declaration,
            [NotNull] HashSet<Name> namespaces)
        {
            if (!(declaration is NamespaceDeclarationSyntax namespaceDeclaration)) return;


            if (namespaceDeclaration.Name != null)
            {
                var namespaceName = nameBuilder.BuildName(namespaceDeclaration.Name);
                if (namespaceName != null)
                {
                    namespaces.Add(namespaceName);
                    containingName = namespaceName;
                }
            }

            foreach (var nestedDeclaration in namespaceDeclaration.Declarations)
                GatherNamespaces(containingName, nestedDeclaration, namespaces);
        }
    }
}
