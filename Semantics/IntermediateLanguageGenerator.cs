using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using ILDeclaration = Adamant.Tools.Compiler.Bootstrap.IL.Declarations.Declaration;
using ILFunctionDeclaration = Adamant.Tools.Compiler.Bootstrap.IL.Declarations.FunctionDeclaration;
using ILPackage = Adamant.Tools.Compiler.Bootstrap.IL.Package;
using ILTypeDeclaration = Adamant.Tools.Compiler.Bootstrap.IL.Declarations.TypeDeclaration;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
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

        private IEnumerable<ILDeclaration> Convert(CompilationUnit compilationUnit)
        {
            return compilationUnit.Declarations.Select(Convert);
        }

        private ILDeclaration Convert(Declaration declaration)
        {
            switch (declaration)
            {
                case ClassDeclaration c:
                    return new ILTypeDeclaration();
                case FunctionDeclaration func:
                    return new ILFunctionDeclaration(func.Name, func.Parameters.Count);
            }
            throw new NotImplementedException();
        }
    }
}
