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
        typeof(IArgumentSyntax),
        typeof(IBodyOrBlockSyntax),
        typeof(ICallableNameSyntax),
        typeof(ICompilationUnitSyntax),
        typeof(IDeclarationSyntax),
        typeof(IElseClauseSyntax),
        typeof(IExpressionSyntax),
        typeof(IParameterSyntax),
        typeof(IReachabilityAnnotationSyntax),
        typeof(IStatementSyntax),
        typeof(ITypeSyntax),
        typeof(IUsingDirectiveSyntax))]
    public partial interface ISyntax
    {
        TextSpan Span { get; }
    }

    public partial interface IAbstractMethodDeclarationSyntax : IMethodDeclarationSyntax
    {
    }

    public partial interface IArgumentSyntax : ISyntax
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

    public partial interface IAssociatedFunctionDeclarationSyntax : IMemberDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        ITypeSyntax? ReturnType { get; }
        DataTypePromise ReturnDataType { get; }
    }

    public partial interface IBinaryOperatorExpressionSyntax : IExpressionSyntax
    {
        BinaryOperator Operator { get; }
    }

    public partial interface IBlockExpressionSyntax : IExpressionSyntax, IBlockOrResultSyntax, IBodyOrBlockSyntax
    {
    }

    [Closed(
        typeof(IBlockExpressionSyntax),
        typeof(IResultStatementSyntax))]
    public partial interface IBlockOrResultSyntax : IElseClauseSyntax
    {
    }

    [Closed(
        typeof(IBlockExpressionSyntax),
        typeof(IBodySyntax))]
    public partial interface IBodyOrBlockSyntax : ISyntax
    {
        FixedList<IStatementSyntax> Statements { get; }
    }

    [Closed(
        typeof(IExpressionStatementSyntax),
        typeof(IVariableDeclarationStatementSyntax))]
    public partial interface IBodyStatementSyntax : IStatementSyntax
    {
    }

    public partial interface IBodySyntax : IBodyOrBlockSyntax
    {
        new FixedList<IBodyStatementSyntax> Statements { get; }
    }

    public partial interface IBoolLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        bool Value { get; }
    }

    public partial interface IBorrowExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface IBreakExpressionSyntax : IExpressionSyntax
    {
    }

    [Closed(
        typeof(IConcreteCallableDeclarationSyntax),
        typeof(IMethodDeclarationSyntax))]
    public partial interface ICallableDeclarationSyntax : IEntityDeclarationSyntax, IFunctionMetadata
    {
        FixedList<IReachabilityAnnotationSyntax> ReachabilityAnnotations { get; }
    }

    public partial interface ICallableNameSyntax : ISyntax
    {
        MaybeQualifiedName Name { get; }
    }

    public partial interface ICanReachAnnotationSyntax : IReachabilityAnnotationSyntax
    {
        FixedList<INameExpressionSyntax> CanReach { get; }
    }

    public partial interface ICapabilityTypeSyntax : ITypeSyntax
    {
        ITypeSyntax ReferentType { get; }
        ReferenceCapability Capability { get; }
    }

    public partial interface IClassDeclarationSyntax : INonMemberEntityDeclarationSyntax
    {
        IMutableKeywordToken? MutableModifier { get; }
        new SimpleName Name { get; }
        FixedList<IMemberDeclarationSyntax> Members { get; }
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

    [Closed(
        typeof(IAssociatedFunctionDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax),
        typeof(IConstructorDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax))]
    public partial interface IConcreteCallableDeclarationSyntax : ICallableDeclarationSyntax
    {
        IBodySyntax Body { get; }
    }

    public partial interface IConcreteMethodDeclarationSyntax : IMethodDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
    }

    public partial interface IConstructorDeclarationSyntax : IMemberDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        ISelfParameterSyntax ImplicitSelfParameter { get; }
    }

    [Closed(
        typeof(IFieldParameterSyntax),
        typeof(INamedParameterSyntax))]
    public partial interface IConstructorParameterSyntax : IParameterSyntax
    {
    }

    [Closed(
        typeof(IEntityDeclarationSyntax),
        typeof(INamespaceDeclarationSyntax),
        typeof(INonMemberDeclarationSyntax))]
    public partial interface IDeclarationSyntax : ISyntax
    {
        CodeFile File { get; }
        TextSpan NameSpan { get; }
        Promise<Symbol?> Symbol { get; }
    }

    [Closed(
        typeof(IBlockOrResultSyntax),
        typeof(IIfExpressionSyntax))]
    public partial interface IElseClauseSyntax : ISyntax
    {
    }

    [Closed(
        typeof(ICallableDeclarationSyntax),
        typeof(IMemberDeclarationSyntax),
        typeof(INonMemberEntityDeclarationSyntax))]
    public partial interface IEntityDeclarationSyntax : IDeclarationSyntax
    {
        IAccessModifierToken? AccessModifier { get; }
        Name? Name { get; }
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

    public partial interface IFieldAccessExpressionSyntax : IAssignableExpressionSyntax
    {
        AccessOperator AccessOperator { get; }
        INameExpressionSyntax Field { get; }
    }

    public partial interface IFieldDeclarationSyntax : IMemberDeclarationSyntax
    {
        ITypeSyntax TypeSyntax { get; }
        DataTypePromise DataType { get; }
    }

    public partial interface IFieldParameterSyntax : IParameterSyntax, IConstructorParameterSyntax
    {
        SimpleName FieldName { get; }
        IExpressionSyntax? DefaultValue { get; }
    }

    public partial interface IForeachExpressionSyntax : IExpressionSyntax
    {
        SimpleName VariableName { get; }
        ITypeSyntax? TypeSyntax { get; }
        IBlockExpressionSyntax Block { get; }
    }

    public partial interface IFunctionDeclarationSyntax : INonMemberEntityDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        bool IsExternalFunction { get; }
        ITypeSyntax? ReturnType { get; }
        DataTypePromise ReturnDataType { get; }
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

    [Closed(
        typeof(IAssociatedFunctionDeclarationSyntax),
        typeof(IConstructorDeclarationSyntax),
        typeof(IFieldDeclarationSyntax),
        typeof(IMethodDeclarationSyntax))]
    public partial interface IMemberDeclarationSyntax : IEntityDeclarationSyntax
    {
        IClassDeclarationSyntax DeclaringClass { get; }
        new SimpleName Name { get; }
    }

    [Closed(
        typeof(IAbstractMethodDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax))]
    public partial interface IMethodDeclarationSyntax : IMemberDeclarationSyntax, ICallableDeclarationSyntax
    {
        ISelfParameterSyntax SelfParameter { get; }
        ITypeSyntax? ReturnType { get; }
        DataTypePromise ReturnDataType { get; }
    }

    public partial interface IMethodInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ICallableNameSyntax MethodNameSyntax { get; }
    }

    public partial interface IMoveExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface INamedParameterSyntax : IParameterSyntax, IConstructorParameterSyntax
    {
        ITypeSyntax TypeSyntax { get; }
        IExpressionSyntax? DefaultValue { get; }
    }

    public partial interface INameExpressionSyntax : IAssignableExpressionSyntax, IHasContainingScope
    {
        SimpleName Name { get; }
    }

    public partial interface INamespaceDeclarationSyntax : INonMemberDeclarationSyntax, IDeclarationSyntax
    {
        bool IsGlobalQualified { get; }
        NamespaceName DeclaredNames { get; }
        NamespaceName Name { get; }
        FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        FixedList<INonMemberDeclarationSyntax> Declarations { get; }
    }

    public partial interface INewObjectExpressionSyntax : IExpressionSyntax
    {
        ITypeNameSyntax TypeSyntax { get; }
        ICallableNameSyntax? ConstructorName { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
    }

    public partial interface INextExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface INoneLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
    }

    [Closed(
        typeof(INamespaceDeclarationSyntax),
        typeof(INonMemberEntityDeclarationSyntax))]
    public partial interface INonMemberDeclarationSyntax : IDeclarationSyntax
    {
        NamespaceName ContainingNamespaceName { get; }
    }

    [Closed(
        typeof(IClassDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax))]
    public partial interface INonMemberEntityDeclarationSyntax : IEntityDeclarationSyntax, INonMemberDeclarationSyntax
    {
        new Name Name { get; }
    }

    public partial interface IOptionalTypeSyntax : ITypeSyntax
    {
        ITypeSyntax Referent { get; }
    }

    [Closed(
        typeof(IConstructorParameterSyntax),
        typeof(IFieldParameterSyntax),
        typeof(INamedParameterSyntax),
        typeof(ISelfParameterSyntax))]
    public partial interface IParameterSyntax : ISyntax, IBindingMetadata
    {
        SimpleName Name { get; }
        bool Unused { get; }
        DataTypePromise DataType { get; }
    }

    [Closed(
        typeof(ICanReachAnnotationSyntax),
        typeof(IReachableFromAnnotationSyntax))]
    public partial interface IReachabilityAnnotationSyntax : ISyntax
    {
    }

    public partial interface IReachableFromAnnotationSyntax : IReachabilityAnnotationSyntax
    {
        FixedList<INameExpressionSyntax> ReachableFrom { get; }
    }

    public partial interface IResultStatementSyntax : IStatementSyntax, IBlockOrResultSyntax
    {
    }

    public partial interface IReturnExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface ISelfExpressionSyntax : IExpressionSyntax, IHasContainingScope
    {
        bool IsImplicit { get; }
    }

    public partial interface ISelfParameterSyntax : IParameterSyntax
    {
        bool MutableSelf { get; }
    }

    public partial interface IShareExpressionSyntax : IExpressionSyntax
    {
        IBindingMetadata SharedSymbol { get; }
    }

    [Closed(
        typeof(IBodyStatementSyntax),
        typeof(IResultStatementSyntax))]
    public partial interface IStatementSyntax : ISyntax
    {
    }

    public partial interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        string Value { get; }
    }

    public partial interface ITypeNameSyntax : ITypeSyntax, IHasContainingScope
    {
    }

    [Closed(
        typeof(ICapabilityTypeSyntax),
        typeof(IOptionalTypeSyntax),
        typeof(ITypeNameSyntax))]
    public partial interface ITypeSyntax : ISyntax
    {
    }

    public partial interface IUnaryOperatorExpressionSyntax : IExpressionSyntax
    {
        UnaryOperatorFixity Fixity { get; }
        UnaryOperator Operator { get; }
    }

    public partial interface IUnsafeExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface IUsingDirectiveSyntax : ISyntax
    {
        NamespaceName Name { get; }
    }

    public partial interface IVariableDeclarationStatementSyntax : IBodyStatementSyntax, IBindingMetadata
    {
        TextSpan NameSpan { get; }
        SimpleName Name { get; }
        ITypeSyntax? TypeSyntax { get; }
        bool InferMutableType { get; }
    }

    public partial interface IWhileExpressionSyntax : IExpressionSyntax
    {
        IBlockExpressionSyntax Block { get; }
    }

}
