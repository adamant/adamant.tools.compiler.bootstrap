using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analayze;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Validation;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        private static readonly TypeSet<SyntaxBranchNode> DeclaredTypeNodeTypes = new TypeSet<SyntaxBranchNode>();
        private static readonly TypeSet<SyntaxBranchNode> DeclarationNodeTypes = new TypeSet<SyntaxBranchNode>();

        static SemanticAnalyzer()
        {
            DeclarationNodeTypes.Add<FunctionDeclarationSyntax>();
            DeclarationNodeTypes.Add<ParameterSyntax>();
            DeclarationNodeTypes.Add<PrimitiveTypeSyntax>();
        }

        private readonly DeclaredTypesAnalyzer declaredTypesAnalyzer = new DeclaredTypesAnalyzer();
        private readonly DeclarationBinder declarationBinder = new DeclarationBinder();
        private readonly AnnotationValidator validator = new AnnotationValidator();

        public Package Analyze(PackageSyntax package)
        {
            var typeAnnotations = new SyntaxAnnotation<DataType>();

            var packageSyntaxSymbol = declaredTypesAnalyzer.GetDeclaredTypes(package, typeAnnotations);

            validator.AssertNodesAreAnnotated(DeclaredTypeNodeTypes, package, typeAnnotations);

            declarationBinder.BindDeclarations(packageSyntaxSymbol, typeAnnotations);

            validator.AssertNodesAreAnnotated(DeclarationNodeTypes, package, typeAnnotations);

            var treeBuilder = new SemanticTreeBuilder(typeAnnotations);

            var compilationUnits = package.SyntaxTrees
#if RELEASE
                .AsParallel()
#endif
                .Select(tree => treeBuilder.Build(tree))
                .ToList();

            var entryPoint = FindEntryPoint(compilationUnits);
            return new Package(package, compilationUnits, entryPoint);
        }

        private FunctionDeclaration FindEntryPoint(IEnumerable<CompilationUnit> compilationUnits)
        {
            return compilationUnits
                .SelectMany(cu => cu.Declarations.OfType<FunctionDeclaration>())
                .Where(f => f.Name == "main")
                .SingleOrDefault();
        }
    }
}
