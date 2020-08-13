using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(ICompilationUnitSyntax),
        typeof(IUsingDirectiveSyntax),
        typeof(ICallableNameSyntax),
        typeof(IArgumentSyntax),
        typeof(IBodyOrBlockSyntax),
        typeof(IElseClauseSyntax),
        typeof(IBindingSyntax),
        typeof(IDeclarationSyntax),
        typeof(IParameterSyntax),
        typeof(IReachabilityAnnotationSyntax),
        typeof(ITypeSyntax),
        typeof(IStatementSyntax),
        typeof(IExpressionSyntax))]
    public partial interface ISyntax
    {
        TextSpan Span { get; }
    }

    public partial interface ICompilationUnitSyntax : ISyntax
    {
        CodeFile CodeFile { get; }
        NamespaceName ImplicitNamespaceName { get; }
        FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        FixedList<INonMemberDeclarationSyntax> Declarations { get; }
        FixedList<IEntityDeclarationSyntax> AllEntityDeclarations { get; }
        FixedList<Diagnostic> Diagnostics { get; }
    }

    public partial interface IUsingDirectiveSyntax : ISyntax
    {
        NamespaceName Name { get; }
    }

    public partial interface ICallableNameSyntax : ISyntax
    {
        MaybeQualifiedName Name { get; }
    }

    public partial interface IArgumentSyntax : ISyntax
    {
    }

    public partial interface IBodySyntax : IBodyOrBlockSyntax
    {
        new FixedList<IBodyStatementSyntax> Statements { get; }
    }

    [Closed(
        typeof(IBodySyntax),
        typeof(IBlockExpressionSyntax))]
    public partial interface IBodyOrBlockSyntax : ISyntax
    {
        FixedList<IStatementSyntax> Statements { get; }
    }

    [Closed(
        typeof(IBlockOrResultSyntax),
        typeof(IIfExpressionSyntax))]
    public partial interface IElseClauseSyntax : ISyntax
    {
    }

    [Closed(
        typeof(IResultStatementSyntax),
        typeof(IBlockExpressionSyntax))]
    public partial interface IBlockOrResultSyntax : IElseClauseSyntax
    {
    }

    [Closed(
        typeof(ILocalBindingSyntax),
        typeof(IFieldDeclarationSyntax))]
    public partial interface IBindingSyntax : ISyntax
    {
        IPromise<BindingSymbol> Symbol { get; }
        DataType BindingDataType { get; }
    }

    [Closed(
        typeof(IBindingParameterSyntax),
        typeof(IVariableDeclarationStatementSyntax),
        typeof(IForeachExpressionSyntax))]
    public partial interface ILocalBindingSyntax : IBindingSyntax
    {
    }

    [Closed(
        typeof(IEntityDeclarationSyntax),
        typeof(INonMemberDeclarationSyntax),
        typeof(INamespaceDeclarationSyntax))]
    public partial interface IDeclarationSyntax : ISyntax, IHasContainingLexicalScope
    {
        CodeFile File { get; }
        TextSpan NameSpan { get; }
    }

    [Closed(
        typeof(ICallableDeclarationSyntax),
        typeof(INonMemberEntityDeclarationSyntax),
        typeof(IMemberDeclarationSyntax))]
    public partial interface IEntityDeclarationSyntax : IDeclarationSyntax
    {
        IAccessModifierToken? AccessModifier { get; }
        Name? Name { get; }
        IPromise<Symbol> Symbol { get; }
    }

    [Closed(
        typeof(IConcreteCallableDeclarationSyntax),
        typeof(IMethodDeclarationSyntax))]
    public partial interface ICallableDeclarationSyntax : IEntityDeclarationSyntax, IFunctionMetadata
    {
        FixedList<IReachabilityAnnotationSyntax> ReachabilityAnnotations { get; }
    }

    [Closed(
        typeof(IFunctionDeclarationSyntax),
        typeof(IAssociatedFunctionDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax),
        typeof(IConstructorDeclarationSyntax))]
    public partial interface IConcreteCallableDeclarationSyntax : ICallableDeclarationSyntax
    {
        IBodySyntax Body { get; }
    }

    [Closed(
        typeof(INamespaceDeclarationSyntax),
        typeof(INonMemberEntityDeclarationSyntax))]
    public partial interface INonMemberDeclarationSyntax : IDeclarationSyntax
    {
        NamespaceName ContainingNamespaceName { get; }
    }

    public partial interface INamespaceDeclarationSyntax : INonMemberDeclarationSyntax, IDeclarationSyntax
    {
        bool IsGlobalQualified { get; }
        NamespaceName DeclaredNames { get; }
        NamespaceName FullName { get; }
        Promise<NamespaceOrPackageSymbol> Symbol { get; }
        FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        FixedList<INonMemberDeclarationSyntax> Declarations { get; }
    }

    [Closed(
        typeof(IClassDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax))]
    public partial interface INonMemberEntityDeclarationSyntax : IEntityDeclarationSyntax, INonMemberDeclarationSyntax
    {
        new Name Name { get; }
    }

    public partial interface IClassDeclarationSyntax : INonMemberEntityDeclarationSyntax
    {
        IMutableKeywordToken? MutableModifier { get; }
        new Name Name { get; }
        new Promise<ObjectTypeSymbol> Symbol { get; }
        FixedList<IMemberDeclarationSyntax> Members { get; }
    }

    public partial interface IFunctionDeclarationSyntax : INonMemberEntityDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        bool IsExternalFunction { get; }
        new Name Name { get; }
        new Promise<FunctionSymbol> Symbol { get; }
        ITypeSyntax? ReturnType { get; }
        Promise<DataType> ReturnDataType { get; }
    }

    public partial interface IAssociatedFunctionDeclarationSyntax : IMemberDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        new Name Name { get; }
        new Promise<FunctionSymbol> Symbol { get; }
        ITypeSyntax? ReturnType { get; }
        Promise<DataType> ReturnDataType { get; }
    }

    [Closed(
        typeof(IAssociatedFunctionDeclarationSyntax),
        typeof(IMethodDeclarationSyntax),
        typeof(IConstructorDeclarationSyntax),
        typeof(IFieldDeclarationSyntax))]
    public partial interface IMemberDeclarationSyntax : IEntityDeclarationSyntax
    {
        IClassDeclarationSyntax DeclaringClass { get; }
    }

    [Closed(
        typeof(IAbstractMethodDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax))]
    public partial interface IMethodDeclarationSyntax : IMemberDeclarationSyntax, ICallableDeclarationSyntax
    {
        new Name Name { get; }
        new Promise<MethodSymbol> Symbol { get; }
        ISelfParameterSyntax SelfParameter { get; }
        ITypeSyntax? ReturnType { get; }
        Promise<DataType> ReturnDataType { get; }
    }

    public partial interface IAbstractMethodDeclarationSyntax : IMethodDeclarationSyntax
    {
    }

    public partial interface IConcreteMethodDeclarationSyntax : IMethodDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
    }

    public partial interface IConstructorDeclarationSyntax : IMemberDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        new Promise<ConstructorSymbol> Symbol { get; }
        ISelfParameterSyntax ImplicitSelfParameter { get; }
    }

    public partial interface IFieldDeclarationSyntax : IMemberDeclarationSyntax, IBindingSyntax
    {
        new Name Name { get; }
        new Promise<FieldSymbol> Symbol { get; }
        ITypeSyntax Type { get; }
    }

    [Closed(
        typeof(IConstructorParameterSyntax),
        typeof(IBindingParameterSyntax),
        typeof(INamedParameterSyntax),
        typeof(ISelfParameterSyntax),
        typeof(IFieldParameterSyntax))]
    public partial interface IParameterSyntax : ISyntax, IBindingMetadata
    {
        Name? Name { get; }
        IPromise<DataType> DataType { get; }
        bool Unused { get; }
    }

    [Closed(
        typeof(INamedParameterSyntax),
        typeof(IFieldParameterSyntax))]
    public partial interface IConstructorParameterSyntax : IParameterSyntax
    {
        new Name Name { get; }
    }

    [Closed(
        typeof(INamedParameterSyntax),
        typeof(ISelfParameterSyntax))]
    public partial interface IBindingParameterSyntax : IParameterSyntax, ILocalBindingSyntax
    {
    }

    public partial interface INamedParameterSyntax : IParameterSyntax, IConstructorParameterSyntax, IBindingParameterSyntax
    {
        Promise<int?> DeclarationNumber { get; }
        ITypeSyntax Type { get; }
        new Promise<VariableSymbol> Symbol { get; }
        IExpressionSyntax? DefaultValue { get; }
    }

    public partial interface ISelfParameterSyntax : IParameterSyntax, IBindingParameterSyntax
    {
        bool MutableSelf { get; }
        new Promise<SelfParameterSymbol> Symbol { get; }
    }

    public partial interface IFieldParameterSyntax : IParameterSyntax, IConstructorParameterSyntax
    {
        Promise<FieldSymbol?> ReferencedSymbol { get; }
        IExpressionSyntax? DefaultValue { get; }
    }

    [Closed(
        typeof(IReachableFromAnnotationSyntax),
        typeof(ICanReachAnnotationSyntax))]
    public partial interface IReachabilityAnnotationSyntax : ISyntax
    {
    }

    public partial interface IReachableFromAnnotationSyntax : IReachabilityAnnotationSyntax
    {
        FixedList<INameExpressionSyntax> ReachableFrom { get; }
    }

    public partial interface ICanReachAnnotationSyntax : IReachabilityAnnotationSyntax
    {
        FixedList<INameExpressionSyntax> CanReach { get; }
    }

    [Closed(
        typeof(ITypeNameSyntax),
        typeof(IOptionalTypeSyntax),
        typeof(ICapabilityTypeSyntax))]
    public partial interface ITypeSyntax : ISyntax
    {
    }

    public partial interface ITypeNameSyntax : ITypeSyntax, IHasContainingScope, IHasContainingLexicalScope
    {
        TypeName Name { get; }
        Promise<TypeSymbol?> ReferencedSymbol { get; }
    }

    public partial interface IOptionalTypeSyntax : ITypeSyntax
    {
        ITypeSyntax Referent { get; }
    }

    public partial interface ICapabilityTypeSyntax : ITypeSyntax
    {
        ITypeSyntax ReferentType { get; }
        ReferenceCapability Capability { get; }
    }

    [Closed(
        typeof(IResultStatementSyntax),
        typeof(IBodyStatementSyntax))]
    public partial interface IStatementSyntax : ISyntax
    {
    }

    public partial interface IResultStatementSyntax : IStatementSyntax, IBlockOrResultSyntax
    {
    }

    [Closed(
        typeof(IVariableDeclarationStatementSyntax),
        typeof(IExpressionStatementSyntax))]
    public partial interface IBodyStatementSyntax : IStatementSyntax
    {
    }

    public partial interface IVariableDeclarationStatementSyntax : IBodyStatementSyntax, ILocalBindingSyntax, IBindingMetadata
    {
        TextSpan NameSpan { get; }
        Name Name { get; }
        Promise<int?> DeclarationNumber { get; }
        new Promise<VariableSymbol> Symbol { get; }
        ITypeSyntax? Type { get; }
        bool InferMutableType { get; }
    }

    public partial interface IExpressionStatementSyntax : IBodyStatementSyntax
    {
    }

    [Closed(
        typeof(IAssignableExpressionSyntax),
        typeof(IAssignmentExpressionSyntax),
        typeof(IBinaryOperatorExpressionSyntax),
        typeof(IBlockExpressionSyntax),
        typeof(IBorrowExpressionSyntax),
        typeof(IBreakExpressionSyntax),
        typeof(IForeachExpressionSyntax),
        typeof(IIfExpressionSyntax),
        typeof(IImplicitConversionExpressionSyntax),
        typeof(IInvocationExpressionSyntax),
        typeof(ILiteralExpressionSyntax),
        typeof(ILoopExpressionSyntax),
        typeof(IMoveExpressionSyntax),
        typeof(INewObjectExpressionSyntax),
        typeof(INextExpressionSyntax),
        typeof(IReturnExpressionSyntax),
        typeof(ISelfExpressionSyntax),
        typeof(IShareExpressionSyntax),
        typeof(IUnaryOperatorExpressionSyntax),
        typeof(IUnsafeExpressionSyntax),
        typeof(IWhileExpressionSyntax))]
    public partial interface IExpressionSyntax : ISyntax
    {
    }

    [Closed(
        typeof(IFieldAccessExpressionSyntax),
        typeof(INameExpressionSyntax))]
    public partial interface IAssignableExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface IAssignmentExpressionSyntax : IExpressionSyntax
    {
        AssignmentOperator Operator { get; }
    }

    public partial interface IBinaryOperatorExpressionSyntax : IExpressionSyntax
    {
        BinaryOperator Operator { get; }
    }

    public partial interface IBlockExpressionSyntax : IExpressionSyntax, IBlockOrResultSyntax, IBodyOrBlockSyntax
    {
    }

    public partial interface IBoolLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        bool Value { get; }
    }

    public partial interface IBorrowExpressionSyntax : IExpressionSyntax
    {
        Promise<BindingSymbol?> ReferencedSymbol { get; }
    }

    public partial interface IBreakExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface IFieldAccessExpressionSyntax : IAssignableExpressionSyntax
    {
        AccessOperator AccessOperator { get; }
        INameExpressionSyntax Field { get; }
        IPromise<FieldSymbol?> ReferencedSymbol { get; }
    }

    public partial interface IForeachExpressionSyntax : IExpressionSyntax, ILocalBindingSyntax
    {
        Name VariableName { get; }
        Promise<int?> DeclarationNumber { get; }
        new Promise<VariableSymbol> Symbol { get; }
        ITypeSyntax? Type { get; }
        IBlockExpressionSyntax Block { get; }
    }

    public partial interface IFunctionInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ICallableNameSyntax FunctionNameSyntax { get; }
    }

    public partial interface IIfExpressionSyntax : IExpressionSyntax, IElseClauseSyntax
    {
        IBlockOrResultSyntax ThenBlock { get; }
        IElseClauseSyntax? ElseClause { get; }
    }

    [Closed(
        typeof(IImplicitImmutabilityConversionExpressionSyntax),
        typeof(IImplicitNoneConversionExpressionSyntax),
        typeof(IImplicitNumericConversionExpressionSyntax),
        typeof(IImplicitOptionalConversionExpressionSyntax))]
    public partial interface IImplicitConversionExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax Expression { get; }
        DataType DataType { get; }
    }

    public partial interface IImplicitImmutabilityConversionExpressionSyntax : IImplicitConversionExpressionSyntax
    {
    }

    public partial interface IImplicitNoneConversionExpressionSyntax : IImplicitConversionExpressionSyntax
    {
        OptionalType ConvertToType { get; }
    }

    public partial interface IImplicitNumericConversionExpressionSyntax : IImplicitConversionExpressionSyntax
    {
        NumericType ConvertToType { get; }
    }

    public partial interface IImplicitOptionalConversionExpressionSyntax : IImplicitConversionExpressionSyntax
    {
        OptionalType ConvertToType { get; }
    }

    public partial interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        BigInteger Value { get; }
    }

    [Closed(
        typeof(IFunctionInvocationExpressionSyntax),
        typeof(IMethodInvocationExpressionSyntax))]
    public partial interface IInvocationExpressionSyntax : IExpressionSyntax
    {
        MaybeQualifiedName FullName { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
    }

    [Closed(
        typeof(IBoolLiteralExpressionSyntax),
        typeof(IIntegerLiteralExpressionSyntax),
        typeof(INoneLiteralExpressionSyntax),
        typeof(IStringLiteralExpressionSyntax))]
    public partial interface ILiteralExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface ILoopExpressionSyntax : IExpressionSyntax
    {
        IBlockExpressionSyntax Block { get; }
    }

    public partial interface IMethodInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ICallableNameSyntax MethodNameSyntax { get; }
    }

    public partial interface IMoveExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface INameExpressionSyntax : IAssignableExpressionSyntax, IHasContainingScope, IHasContainingLexicalScope
    {
        Name? Name { get; }
        SimpleName SimpleName { get; }
        Promise<BindingSymbol?> ReferencedSymbol { get; }
    }

    public partial interface INewObjectExpressionSyntax : IExpressionSyntax
    {
        ITypeNameSyntax Type { get; }
        ICallableNameSyntax? ConstructorName { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
    }

    public partial interface INextExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface INoneLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
    }

    public partial interface IReturnExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface ISelfExpressionSyntax : IExpressionSyntax, IHasContainingScope
    {
        bool IsImplicit { get; }
        Promise<SelfParameterSymbol?> ReferencedSymbol { get; }
    }

    public partial interface IShareExpressionSyntax : IExpressionSyntax
    {
        IBindingMetadata SharedMetadata { get; }
    }

    public partial interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        string Value { get; }
    }

    public partial interface IUnaryOperatorExpressionSyntax : IExpressionSyntax
    {
        UnaryOperatorFixity Fixity { get; }
        UnaryOperator Operator { get; }
    }

    public partial interface IUnsafeExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface IWhileExpressionSyntax : IExpressionSyntax
    {
        IBlockExpressionSyntax Block { get; }
    }

}
