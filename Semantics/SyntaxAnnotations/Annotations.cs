using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations
{
    public class Annotations
    {
        private static readonly TypeSet<SyntaxBranchNode> NodeTypesWithSymbols = new TypeSet<SyntaxBranchNode>();
        private static readonly TypeSet<SyntaxBranchNode> NodeTypesWithScopes = new TypeSet<SyntaxBranchNode>();
        private static readonly TypeSet<SyntaxBranchNode> NodeTypesWithDataTypes = new TypeSet<SyntaxBranchNode>();
        private static readonly TypeSet<SyntaxBranchNode> NodeTypesWithNames = new TypeSet<SyntaxBranchNode>();
        private static readonly AnnotationValidator Validator = new AnnotationValidator();

        static Annotations()
        {
            NodeTypesWithSymbols.Add<FunctionDeclarationSyntax>();
            NodeTypesWithSymbols.Add<ParameterSyntax>();
            NodeTypesWithSymbols.Add<CompilationUnitSyntax>();

            NodeTypesWithScopes.Add<CompilationUnitSyntax>();
            NodeTypesWithScopes.Add<FunctionDeclarationSyntax>();
            NodeTypesWithScopes.Add<IdentifierNameSyntax>();
            NodeTypesWithScopes.Add<ParameterSyntax>();

            NodeTypesWithDataTypes.Add<FunctionDeclarationSyntax>();
            NodeTypesWithDataTypes.Add<ParameterSyntax>();
            NodeTypesWithDataTypes.Add<PrimitiveTypeSyntax>();
            NodeTypesWithDataTypes.Add<IdentifierNameSyntax>();
            NodeTypesWithDataTypes.Add<BinaryOperatorExpressionSyntax>();

            NodeTypesWithNames.Add<PackageSyntax>();
            NodeTypesWithNames.Add<IdentifierNameSyntax>();
            NodeTypesWithNames.Add<ParameterSyntax>();
            NodeTypesWithNames.Add<FunctionDeclarationSyntax>();
            NodeTypesWithNames.Add<CompilationUnitSyntax>();
        }

        public PackageSyntax Package { get; }
        private readonly SyntaxAnnotation<ISyntaxSymbol> symbols;
        private readonly SyntaxAnnotation<DataType> oldTypes = new SyntaxAnnotation<DataType>();
        private readonly SyntaxAnnotation<NameScope> scopes = new SyntaxAnnotation<NameScope>();
        private readonly SyntaxAnnotation<Lazy<DataType>> types = new SyntaxAnnotation<Lazy<DataType>>();
        private readonly SyntaxAnnotation<Lazy<Name>> names = new SyntaxAnnotation<Lazy<Name>>();

        public Annotations(PackageSyntax package, SyntaxAnnotation<ISyntaxSymbol> symbols)
        {
            this.symbols = symbols;
            Package = package;
        }

        #region Validations
        public void ValidateSymbolsAnnotations()
        {
            Validator.AssertNodesAreAnnotated(NodeTypesWithSymbols, Package, symbols);
        }

        public void ValidateNameScopeAnnotations()
        {
            Validator.AssertNodesAreAnnotated(NodeTypesWithScopes, Package, scopes);
        }

        public void ValidateAnnotationBindings()
        {
            Validator.AssertNodesAreAnnotated(NodeTypesWithDataTypes, Package, types);
            Validator.AssertNodesAreAnnotated(NodeTypesWithNames, Package, names);
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

        public void Add(SyntaxBranchNode syntax, Lazy<DataType> type)
        {
            types.Add(syntax, type);
        }

        public void Add(SyntaxBranchNode syntax, Lazy<Name> name)
        {
            names.Add(syntax, name);
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

        public NameScope Scope(SyntaxBranchNode syntax)
        {
            return scopes[syntax];
        }

        public DataType Type(SyntaxBranchNode syntax)
        {
            return types[syntax].Value;
        }

        public Name Name(SyntaxBranchNode syntax)
        {
            return names[syntax].Value;
        }
        #endregion
    }
}
