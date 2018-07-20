using System;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analayze;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        public Package Analyze(PackageSyntax package)
        {
            var declaredTypesAnalyzer = new DeclaredTypesAnalyzer();
            var packageSyntaxSymbol = declaredTypesAnalyzer.GetDeclaredTypes(package);

            var typeAnnotations = new SyntaxAnnotation<Type>();

            var declarationBinder = new DeclarationBinder();
            declarationBinder.BindDeclarations(packageSyntaxSymbol, typeAnnotations);

            throw new NotImplementedException();
        }
    }
}
