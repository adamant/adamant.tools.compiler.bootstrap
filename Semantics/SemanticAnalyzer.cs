using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        public Package Analyze([NotNull] PackageSyntax packageSyntax)
        {
            var diagnostics = new DiagnosticsBuilder();
            var package = new Package(packageSyntax.Name);
            DeclarationBuilder.GatherDeclarations(package, packageSyntax);

            // Hack for now
            foreach (var function in package.Declarations.OfType<FunctionDeclaration>())
                function.ReturnType = ObjectType.Void;

            DetermineEntryPoint(package, diagnostics);
            package.Diagnostics = diagnostics.Build();
            return package;
        }

        private static void DetermineEntryPoint([NotNull] Package package, [NotNull] DiagnosticsBuilder diagnostics)
        {
            var mainFunctions = package.Declarations.OfType<FunctionDeclaration>()
                // TODO make an easy way to construct and compare qualified names
                .Where(f => !f.QualifiedName.Qualifier.Any()
                            && f.QualifiedName.Name.Text == "main")
                .ToList();

            // TODO warn on and remove main functions that don't have correct parameters or types

            // TODO compiler error on multiple main functions

            package.EntryPoint = mainFunctions.SingleOrDefault();
        }
    }
}
