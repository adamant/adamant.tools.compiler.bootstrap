using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using ILDeclaration = Adamant.Tools.Compiler.Bootstrap.IL.Declarations.Declaration;
using ILFunctionDeclaration = Adamant.Tools.Compiler.Bootstrap.IL.Declarations.FunctionDeclaration;
using ILPackage = Adamant.Tools.Compiler.Bootstrap.IL.Package;
using ILTypeDeclaration = Adamant.Tools.Compiler.Bootstrap.IL.Declarations.TypeDeclaration;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IL
{
    public class IntermediateLanguageGenerator
    {
        public ILPackage Convert(Package package)
        {
            var ilPackage = new ILPackage("default"); // TODO use the real package name
            foreach (var declaration in package.CompilationUnits.SelectMany(Convert))
                ilPackage.Add(declaration);

            return ilPackage;
        }

        private static IEnumerable<ILDeclaration> Convert(CompilationUnit compilationUnit)
        {
            return compilationUnit.Declarations
                .Select(Convert)
                // Incomplete Declarations Return Null
                .Where(d => d != null);
        }

        private static ILDeclaration Convert(Declaration declaration)
        {
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

        private static ILTypeDeclaration Convert(ClassDeclaration @class)
        {
            return new ILTypeDeclaration(@class.Name, true);
        }

        private static ILTypeDeclaration Convert(EnumStructDeclaration enumStruct)
        {
            return new ILTypeDeclaration(enumStruct.Name, false);
        }

        private static ILFunctionDeclaration Convert(FunctionDeclaration function)
        {
            return new FunctionIntermediateLanguageGenerator(function).Convert();
        }
    }
}
