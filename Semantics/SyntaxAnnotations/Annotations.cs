using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations
{
    internal class Annotations
    {
        private static readonly TypeSet<SyntaxBranchNode> NodesTypesWithSyntaxSymbols = new TypeSet<SyntaxBranchNode>();
        private static readonly TypeSet<SyntaxBranchNode> DeclarationNodeTypes = new TypeSet<SyntaxBranchNode>();
        private static readonly AnnotationValidator Validator = new AnnotationValidator();

        static Annotations()
        {
            NodesTypesWithSyntaxSymbols.Add<FunctionDeclarationSyntax>();
            NodesTypesWithSyntaxSymbols.Add<ParameterSyntax>();

            DeclarationNodeTypes.Add<FunctionDeclarationSyntax>();
            DeclarationNodeTypes.Add<ParameterSyntax>();
            DeclarationNodeTypes.Add<PrimitiveTypeSyntax>();
        }

        private readonly PackageSyntax package;
        private readonly SyntaxAnnotation<ISyntaxSymbol> symbols;
        private readonly SyntaxAnnotation<DataType> types = new SyntaxAnnotation<DataType>();

        public Annotations(PackageSyntax package, SyntaxAnnotation<ISyntaxSymbol> symbols)
        {
            this.symbols = symbols;
            this.package = package;
        }

        public DataType Type(SyntaxBranchNode syntax)
        {
            return types[syntax];
        }

        public void ValidateSymbolsAnnotations()
        {
            Validator.AssertNodesAreAnnotated(NodesTypesWithSyntaxSymbols, package, symbols);
        }

        public void ValidateAnnotations()
        {
            Validator.AssertNodesAreAnnotated(DeclarationNodeTypes, package, types);
        }

        public void Add(SyntaxBranchNode syntax, DataType type)
        {
            types.Add(syntax, type);
        }

        public ISyntaxSymbol Symbol(SyntaxBranchNode syntax)
        {
            return symbols[syntax];
        }

        public IPackageSyntaxSymbol Symbol(PackageSyntax syntax)
        {
            return (IPackageSyntaxSymbol)symbols[syntax];
        }
    }
}
