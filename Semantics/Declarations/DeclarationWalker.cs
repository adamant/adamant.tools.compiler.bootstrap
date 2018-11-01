using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Namespaces;
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
                GatherNamespaces(compilationUnit.CodeFile, GlobalNamespaceName.Instance, compilationUnit.Namespace, namespaces);

            return namespaces.Select(ns => new Namespace(ns)).ToReadOnlyList();
        }

        private void GatherNamespaces(
            [NotNull] CodeFile codeFile,
            [NotNull] Name namespaceName,
            [NotNull] DeclarationSyntax declaration,
            [NotNull] HashSet<Name> namespaces)
        {
            if (!(declaration is NamespaceDeclarationSyntax namespaceDeclaration)) return;

            if (namespaceDeclaration.Name != null)
                namespaceName = nameBuilder.BuildName(namespaceDeclaration.Name) ?? namespaceName;

            namespaces.Add(namespaceName);

            foreach (var nestedDeclaration in namespaceDeclaration.Declarations)
                GatherNamespaces(codeFile, namespaceName, nestedDeclaration, namespaces);
        }
    }
}
