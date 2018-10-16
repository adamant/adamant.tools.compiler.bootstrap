using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class DeclarationBuilder
    {
        [NotNull] private readonly NameBuilder nameBuilder;

        public DeclarationBuilder([NotNull] NameBuilder nameBuilder)
        {
            this.nameBuilder = nameBuilder;
        }

        [NotNull]
        [ItemNotNull]
        public IList<CompilationUnitScope> GatherDeclarations(
            [NotNull] Package package,
            [NotNull] PackageSyntax packageSyntax)
        {
            return GatherDeclarationsEnumerable(package, packageSyntax).ToList();
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<CompilationUnitScope> GatherDeclarationsEnumerable(
            [NotNull] Package package,
            [NotNull] PackageSyntax packageSyntax)
        {
            foreach (var compilationUnit in packageSyntax.CompilationUnits)
            {
                var globalScope = new CompilationUnitScope();
                Name @namespace = GlobalNamespaceName.Instance;
                if (compilationUnit.Namespace != null)
                    @namespace = nameBuilder.BuildName(compilationUnit.Namespace.Name) ?? @namespace;

                foreach (var declaration in compilationUnit.Declarations)
                    GatherDeclarations(package, compilationUnit.CodeFile, @namespace, declaration);

                yield return globalScope;
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
                case EnumStructDeclarationSyntax enumStruct:
                    BuildEnumStruct(package, codeFile, @namespace, enumStruct);
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
            // Skip any class that doesn't have a name
            if (@class.Name?.Value is string name)
            {
                var fullName = @namespace.Qualify(name);
                package.Add(new TypeDeclaration(codeFile, fullName));
            }
        }

        private static void BuildEnumStruct(
            [NotNull] Package package,
            [NotNull] CodeFile codeFile,
            [NotNull] Name @namespace,
            [NotNull] EnumStructDeclarationSyntax enumStruct)
        {
            // Skip any struct that doesn't have a name
            if (enumStruct.Name?.Value is string name)
            {
                var fullName = @namespace.Qualify(name);
                package.Add(new TypeDeclaration(codeFile, fullName));
            }
        }
    }
}
