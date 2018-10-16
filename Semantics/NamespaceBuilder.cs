using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class NamespaceBuilder
    {
        [NotNull] private readonly NameBuilder nameBuilder;

        public NamespaceBuilder([NotNull] NameBuilder nameBuilder)
        {
            this.nameBuilder = nameBuilder;
        }

        public void GatherNamespaces(
            [NotNull] Package package,
            [NotNull] PackageSyntax packageSyntax)
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
                    GatherNamespaces(package, compilationUnit.CodeFile, @namespace, declaration);
            }

            foreach (var @namespace in namespaces)
            {
                package.Add(new Namespace(@namespace));
            }
        }

        private void GatherNamespaces(Package package, CodeFile codeFile, Name @namespace, NamespaceDeclarationSyntax declaration)
        {
            throw new NotImplementedException();
        }
    }
}
