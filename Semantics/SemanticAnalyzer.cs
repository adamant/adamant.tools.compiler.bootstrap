using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        public Package Analyze([NotNull] PackageSyntax packageSyntax)
        {
            var diagnostics = new DiagnosticsBuilder();
            var package = new Package(packageSyntax.Name);
            GatherDeclarations(package, packageSyntax);

            // Hack for now
            foreach (var function in package.Declarations.OfType<FunctionDeclaration>())
                function.ReturnType = ObjectType.Void;

            DetermineEntryPoint(package, diagnostics);
            package.Diagnostics = diagnostics.Build();
            return package;
        }

        private static void GatherDeclarations(
            [NotNull] Package package,
            [NotNull] PackageSyntax packageSyntax)
        {
            foreach (var compilationUnit in packageSyntax.CompilationUnits)
            {
                if (compilationUnit.Namespace != null)
                {
                    throw new NotImplementedException();
                }

                foreach (var declaration in compilationUnit.Declarations)
                {
                    GatherDeclarations(package, compilationUnit.CodeFile, declaration);
                }
            }
        }

        private static void GatherDeclarations(
            [NotNull] Package package,
            [NotNull] CodeFile codeFile,
            [NotNull] DeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax function:
                    // Skip any function that doesn't have a name
                    if (function.Name?.Value is string name)
                    {
                        var fullName = new QualifiedName(name);
                        package.Add(new FunctionDeclaration(codeFile, fullName));
                    }
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private static void DetermineEntryPoint(Package package, DiagnosticsBuilder diagnostics)
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
