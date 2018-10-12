using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class DeclarationBuilder
    {
        public void GatherDeclarations(
            [NotNull] Package package,
            [NotNull] PackageSyntax packageSyntax)
        {
            foreach (var compilationUnit in packageSyntax.CompilationUnits)
            {
                if (compilationUnit.Namespace != null)
                    throw new NotImplementedException();

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
                    BuildFunction(package, codeFile, function);
                    break;
                case ClassDeclarationSyntax @class:
                    BuildClass(package, codeFile, @class);
                    break;
                case IncompleteDeclarationSyntax _:
                    // Since it is incomplete, we can't do any analysis on it
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private static void BuildFunction(
            [NotNull] Package package,
            [NotNull] CodeFile codeFile,
            [NotNull] FunctionDeclarationSyntax function)
        {
            // Skip any function that doesn't have a name
            if (function.Name?.Value is string name)
            {
                var fullName = new QualifiedName(name);
                package.Add(new FunctionDeclaration(codeFile, fullName));
            }
        }

        private static void BuildClass(
            [NotNull] Package package,
            [NotNull] CodeFile codeFile,
            [NotNull] ClassDeclarationSyntax @class)
        {
            // Skip any function that doesn't have a name
            if (@class.Name?.Value is string name)
            {
                var fullName = new QualifiedName(name);
                package.Add(new TypeDeclaration(codeFile, fullName));
            }
        }
    }
}
