using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IL;
using Adamant.Tools.Compiler.Bootstrap.IL.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IL
{
    public class IntermediateLanguageGenerator
    {
        [NotNull]
        public PackageIL Convert([NotNull] Package package)
        {
            Requires.NotNull(nameof(package), package);
            var ilPackage = new PackageIL("default"); // TODO use the real package name
            foreach (var declaration in package.CompilationUnits.SelectMany(Convert))
                ilPackage.Add(declaration);

            return ilPackage;
        }

        private static IEnumerable<DeclarationIL> Convert([NotNull] CompilationUnit compilationUnit)
        {
            Requires.NotNull(nameof(compilationUnit), compilationUnit);
            return compilationUnit.Declarations
                .Select(Convert)
                // Incomplete Declarations Return Null
                .Where(d => d != null);
        }

        private static DeclarationIL Convert([NotNull] Declaration declaration)
        {
            Requires.NotNull(nameof(declaration), declaration);
            switch (declaration)
            {
                case ClassDeclaration @class:
                    return Convert(@class);
                case EnumStructDeclaration enumStruct:
                    return Convert(enumStruct);
                case FunctionDeclaration function:
                    return Convert(function);
                case IncompleteDeclaration _:
                    return null;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private static TypeDeclarationIL Convert([NotNull] ClassDeclaration @class)
        {
            Requires.NotNull(nameof(@class), @class);
            return new TypeDeclarationIL(@class.Name, true);
        }

        private static TypeDeclarationIL Convert([NotNull] EnumStructDeclaration enumStruct)
        {
            Requires.NotNull(nameof(enumStruct), enumStruct);
            return new TypeDeclarationIL(enumStruct.Name, false);
        }

        [NotNull]
        private static FunctionDeclarationIL Convert([NotNull] FunctionDeclaration function)
        {
            return new FunctionIntermediateLanguageGenerator(function).Convert();
        }
    }
}
