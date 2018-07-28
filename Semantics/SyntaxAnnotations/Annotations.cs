using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations
{
    internal class Annotations
    {
        private static readonly TypeSet<SyntaxBranchNode> NodesTypesWithSymbols = new TypeSet<SyntaxBranchNode>();
        private static readonly TypeSet<SyntaxBranchNode> NodesTypesWithScopes = new TypeSet<SyntaxBranchNode>();
        private static readonly TypeSet<SyntaxBranchNode> DeclarationNodeTypes = new TypeSet<SyntaxBranchNode>();
        private static readonly AnnotationValidator Validator = new AnnotationValidator();

        static Annotations()
        {
            NodesTypesWithSymbols.Add<FunctionDeclarationSyntax>();
            NodesTypesWithSymbols.Add<ParameterSyntax>();
            NodesTypesWithSymbols.Add<CompilationUnitSyntax>();

            NodesTypesWithScopes.Add<CompilationUnitSyntax>();
            NodesTypesWithScopes.Add<FunctionDeclarationSyntax>();

            DeclarationNodeTypes.Add<FunctionDeclarationSyntax>();
            DeclarationNodeTypes.Add<ParameterSyntax>();
            DeclarationNodeTypes.Add<PrimitiveTypeSyntax>();
        }

        private readonly PackageSyntax package;
        private readonly SyntaxAnnotation<ISyntaxSymbol> symbols;
        private readonly SyntaxAnnotation<DataType> oldTypes = new SyntaxAnnotation<DataType>();
        private readonly SyntaxAnnotation<NameScope> scopes = new SyntaxAnnotation<NameScope>();
        private readonly SyntaxAnnotation<Lazy<DataType>> types = new SyntaxAnnotation<Lazy<DataType>>();

        public Annotations(PackageSyntax package, SyntaxAnnotation<ISyntaxSymbol> symbols)
        {
            this.symbols = symbols;
            this.package = package;
        }

        #region Validations
        public void ValidateSymbolsAnnotations()
        {
            Validator.AssertNodesAreAnnotated(NodesTypesWithSymbols, package, symbols);
        }

        public void ValidateNameScopeAnnotations()
        {
            Validator.AssertNodesAreAnnotated(NodesTypesWithScopes, package, scopes);
        }

        public void ValidateOldTypeAnnotations()
        {
            Validator.AssertNodesAreAnnotated(DeclarationNodeTypes, package, oldTypes);
        }

        public void ValidateTypeAnnotations()
        {
            Validator.AssertNodesAreAnnotated(DeclarationNodeTypes, package, types);
        }
        #endregion

        #region Add
        public void Add(SyntaxBranchNode syntax, NameScope scope)
        {
            scopes.Add(syntax, scope);
        }

        public void Add(SyntaxBranchNode syntax, DataType type)
        {
            oldTypes.Add(syntax, type);
        }

        internal void Add(SyntaxBranchNode syntax, Lazy<DataType> type)
        {
            types.Add(syntax, type);
        }
        #endregion

        #region Get
        public ISyntaxSymbol Symbol(SyntaxBranchNode syntax)
        {
            return symbols[syntax];
        }

        public IPackageSyntaxSymbol Symbol(PackageSyntax syntax)
        {
            return (IPackageSyntaxSymbol)symbols[syntax];
        }

        public IGlobalNamespaceSyntaxSymbol Symbol(CompilationUnitSyntax syntax)
        {
            return (IGlobalNamespaceSyntaxSymbol)symbols[syntax];
        }

        public DataType Type(SyntaxBranchNode syntax)
        {
            return oldTypes[syntax];
        }
        #endregion
    }
}
