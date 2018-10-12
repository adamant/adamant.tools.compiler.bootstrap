using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
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
                Name @namespace = GlobalNamespaceName.Instance;
                if (compilationUnit.Namespace != null)
                {
                    @namespace = BuildName(compilationUnit.Namespace.Name) ?? @namespace;
                }

                foreach (var declaration in compilationUnit.Declarations)
                    GatherDeclarations(package, compilationUnit.CodeFile, @namespace, declaration);
            }
        }

        private static void GatherDeclarations(
            [NotNull] Package package,
            [NotNull] CodeFile codeFile,
            [NotNull] Name @namespace,
            [NotNull] DeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax function:
                    BuildFunction(package, codeFile, @namespace, function);
                    break;
                case ClassDeclarationSyntax @class:
                    BuildClass(package, codeFile, @namespace, @class);
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
            [NotNull] Name @namespace,
            [NotNull] FunctionDeclarationSyntax function)
        {
            // Skip any function that doesn't have a name
            if (function.Name?.Value is string name)
            {
                var fullName = @namespace.Qualify(name);
                package.Add(new FunctionDeclaration(codeFile, fullName));
            }
        }

        private static void BuildClass(
            [NotNull] Package package,
            [NotNull] CodeFile codeFile,
            [NotNull] Name @namespace,
            [NotNull] ClassDeclarationSyntax @class)
        {
            // Skip any function that doesn't have a name
            if (@class.Name?.Value is string name)
            {
                var fullName = @namespace.Qualify(name);
                package.Add(new TypeDeclaration(codeFile, fullName));
            }
        }

        [CanBeNull]
        private static Name BuildName([NotNull] NameSyntax nameSyntax)
        {
            switch (nameSyntax)
            {
                case QualifiedNameSyntax qualifiedNameSyntax:
                    {
                        var qualifier = BuildName(qualifiedNameSyntax.Qualifier);
                        var name = qualifiedNameSyntax.Name.Name?.Value;
                        return name != null ? qualifier?.Qualify(name) ?? (Name)new SimpleName(name) : null;
                    }
                case IdentifierNameSyntax identifierNameSyntax:
                    {
                        var name = identifierNameSyntax.Name?.Value;
                        return name != null ? new SimpleName(name) : null;
                    }
                default:
                    throw NonExhaustiveMatchException.For(nameSyntax);
            }
        }
    }
}
