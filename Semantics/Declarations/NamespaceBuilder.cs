using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
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
        public IEnumerable<Namespace> GatherNamespaces([NotNull] PackageSyntax packageSyntax)
        {
            var namespaces = new HashSet<Name>();

            foreach (var compilationUnit in packageSyntax.CompilationUnits)
            {
                Name @namespace = GlobalNamespaceName.Instance;
                if (compilationUnit.Namespace != null)
                    @namespace = nameBuilder.BuildName(compilationUnit.Namespace.Name) ??
                                 @namespace;

                namespaces.Add(@namespace);

                foreach (var declaration in compilationUnit.Declarations.OfType<NamespaceDeclarationSyntax>())
                    GatherNamespaces(compilationUnit.CodeFile, @namespace, declaration);
            }

            foreach (var @namespace in namespaces)
            {
                yield return new Namespace(@namespace);
            }
        }

        private void GatherNamespaces(
            CodeFile codeFile,
            Name @namespace,
            NamespaceDeclarationSyntax declaration)
        {
            throw new NotImplementedException();
        }
    }
}
