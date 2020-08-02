using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST.Gen
{
    [Closed(
        typeof(IArgumentSyntax),
        typeof(IBodyOrBlockSyntax),
        typeof(IBodyStatementSyntax),
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
        IExpressionSyntax Expression { get; }
    }

    [Closed(
        typeof(IFieldAccessExpressionSyntax),
        typeof(INameExpressionSyntax))]
    public partial interface IAssignableExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface IAssignmentExpressionSyntax : IExpressionSyntax
    {
        IAssignableExpressionSyntax LeftOperand { get; }
        AssignmentOperator Operator { get; }
        IExpressionSyntax RightOperand { get; }
    }

    public partial interface IAssociatedFunctionDeclarationSyntax : IMemberDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        new FixedList<INamedParameterSyntax> NewParameters { get; }
        ITypeSyntax? ReturnTypeSyntax { get; }
        DataTypePromise ReturnType { get; }
    }

    public partial interface IBinaryOperatorExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax LeftOperand { get; }
        BinaryOperator Operator { get; }
        IExpressionSyntax RightOperand { get; }
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
    public partial interface IBodyStatementSyntax : ISyntax
    {
        IStatementSyntax Statement { get; }
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
        IExpressionSyntax Referent { get; }
        IBindingMetadata BorrowedFromBinding { get; }
    }

    public partial interface IBreakExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax Value { get; }
    }

    [Closed(
        typeof(IConcreteCallableDeclarationSyntax),
        typeof(IMethodDeclarationSyntax))]
    public partial interface ICallableDeclarationSyntax : IEntityDeclarationSyntax, IFunctionMetadata
    {
        FixedList<IParameterSyntax> NewParameters { get; }
        FixedList<IReachabilityAnnotationSyntax> ReachabilityAnnotations { get; }
    }

    public partial interface ICallableNameSyntax : ISyntax
    {
        Name Name { get; }
        IFunctionMetadata? ReferencedFunctionMetadata { get; }
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
        SimpleName Name { get; }
        FixedList<IMemberDeclarationSyntax> Members { get; }
        DataTypePromise DeclaresType { get; }
    }

    public partial interface ICompilationUnitSyntax : ISyntax
    {
        CodeFile File { get; }
        RootName ImplicitNamespaceName { get; }
        FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        FixedList<INonMemberDeclarationSyntax> Declarations { get; }
        FixedList<IEntityDeclarationSyntax> EntityDeclarations { get; }
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
        DataType? SelfParameterType { get; }
        new IConstructorParameterSyntax NewParameters { get; }
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
    }

    public partial interface IExpressionStatementSyntax : IBodyStatementSyntax
    {
        IExpressionSyntax Expression { get; }
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
        DataType? Type { get; }
        ExpressionSemantics? Semantics { get; }
    }

    public partial interface IFieldAccessExpressionSyntax : IAssignableExpressionSyntax
    {
        IExpressionSyntax ContextExpression { get; }
        AccessOperator AccessOperator { get; }
        INameExpressionSyntax Field { get; }
        IBindingMetadata? ReferencedBinding { get; }
    }

    public partial interface IFieldDeclarationSyntax : IMemberDeclarationSyntax
    {
        ITypeSyntax TypeSyntax { get; }
        DataTypePromise Type { get; }
        IExpressionSyntax? Initializer { get; }
    }

    public partial interface IFieldParameterSyntax : IConstructorParameterSyntax
    {
        SimpleName FieldName { get; }
        IExpressionSyntax? DefaultValue { get; }
    }

    public partial interface IForeachExpressionSyntax : IExpressionSyntax
    {
        SimpleName VariableName { get; }
        bool VariableIsLiveAfterAssignment { get; }
        ITypeSyntax TypeSyntax { get; }
        new DataType? Type { get; }
        DataType? VariableType { get; }
        IExpressionSyntax InExpression { get; }
        IBlockExpressionSyntax Block { get; }
    }

    public partial interface IFunctionDeclarationSyntax : INonMemberEntityDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        bool IsExternalFunction { get; }
        new FixedList<INamedParameterSyntax> NewParameters { get; }
        ITypeSyntax? ReturnTypeSyntax { get; }
        DataTypePromise ReturnType { get; }
    }

    public partial interface IIfExpressionSyntax : IExpressionSyntax, IElseClauseSyntax
    {
        IExpressionSyntax Condition { get; }
        IBlockOrResultSyntax ThenBlock { get; }
        IElseClauseSyntax ElseClause { get; }
    }

    [Closed(
        typeof(IImplicitImmutabilityConversionExpressionSyntax),
        typeof(IImplicitNoneConversionExpressionSyntax),
        typeof(IImplicitNumericConversionExpressionSyntax),
        typeof(IImplicitOptionalConversionExpressionSyntax))]
    public partial interface IImplicitConversionExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax Expression { get; }
        new DataType Type { get; }
    }

    public partial interface IImplicitImmutabilityConversionExpressionSyntax : IImplicitConversionExpressionSyntax
    {
    }

    public partial interface IImplicitNoneConversionExpressionSyntax : IImplicitConversionExpressionSyntax
    {
    }

    public partial interface IImplicitNumericConversionExpressionSyntax : IImplicitConversionExpressionSyntax
    {
        NumericType ConvertToType { get; }
    }

    public partial interface IImplicitOptionalConversionExpressionSyntax : IImplicitConversionExpressionSyntax
    {
        IOptionalTypeSyntax ConvertToType { get; }
    }

    public partial interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        BigInteger Value { get; }
    }

    [Closed(
        typeof(IMethodInvocationExpressionSyntax))]
    public partial interface IInvocationExpressionSyntax : IExpressionSyntax
    {
        Name FullName { get; }
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
        SimpleName Name { get; }
    }

    [Closed(
        typeof(IAbstractMethodDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax))]
    public partial interface IMethodDeclarationSyntax : IMemberDeclarationSyntax, ICallableDeclarationSyntax
    {
        ISelfParameterSyntax SelfParameter { get; }
        DataType? SelfParameterType { get; }
        new FixedList<IParameterSyntax> NewParameters { get; }
        ITypeSyntax ReturnTypeSyntax { get; }
        DataTypePromise ReturnType { get; }
    }

    public partial interface IMethodInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        IExpressionSyntax ContextExpression { get; }
        ICallableNameSyntax MethodNameSyntax { get; }
    }

    public partial interface IMoveExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax Referent { get; }
        IBindingMetadata? MovedSymbol { get; }
    }

    public partial interface INamedParameterSyntax : IParameterSyntax, IConstructorParameterSyntax
    {
        ITypeSyntax TypeSyntax { get; }
        IExpressionSyntax? DefaultValue { get; }
    }

    public partial interface INameExpressionSyntax : IAssignableExpressionSyntax, IHasContainingScope
    {
        SimpleName Name { get; }
        IBindingMetadata? ReferencedBinding { get; }
        bool VariableIsLiveAfter { get; }
    }

    public partial interface INamespaceDeclarationSyntax : INonMemberDeclarationSyntax, IDeclarationSyntax
    {
        bool IsGlobalQualified { get; }
        Name Name { get; }
        Name FullName { get; }
        FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        FixedList<INonMemberDeclarationSyntax> Declarations { get; }
    }

    public partial interface INewObjectExpressionSyntax : IExpressionSyntax
    {
        ITypeNameSyntax TypeSyntax { get; }
        ICallableNameSyntax? ConstructorName { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
        IFunctionMetadata? ReferencedConstructor { get; }
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
    }

    [Closed(
        typeof(IClassDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax))]
    public partial interface INonMemberEntityDeclarationSyntax : IEntityDeclarationSyntax, INonMemberDeclarationSyntax
    {
    }

    public partial interface IOptionalTypeSyntax : ITypeSyntax
    {
        ITypeSyntax Referent { get; }
    }

    [Closed(
        typeof(IConstructorParameterSyntax),
        typeof(INamedParameterSyntax),
        typeof(ISelfParameterSyntax))]
    public partial interface IParameterSyntax : ISyntax, IBindingMetadata
    {
        SimpleName Name { get; }
        bool Unused { get; }
        DataTypePromise Type { get; }
    }

    [Closed(
        typeof(ICanReachAnnotationSyntax),
        typeof(IReachableFromAnnotationSyntax))]
    public partial interface IReachabilityAnnotationSyntax : ISyntax
    {
    }

    public partial interface IReachableFromAnnotationSyntax : IReachabilityAnnotationSyntax
    {
        INameExpressionSyntax ReachableFrom { get; }
    }

    public partial interface IResultStatementSyntax : IStatementSyntax, IBlockOrResultSyntax
    {
        IExpressionSyntax Expression { get; }
    }

    public partial interface IReturnExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax? ReturnValue { get; }
    }

    public partial interface ISelfExpressionSyntax : IExpressionSyntax, IHasContainingScope
    {
        bool IsImplicit { get; }
        IBindingMetadata? ReferencedBinding { get; }
    }

    public partial interface ISelfParameterSyntax : IParameterSyntax
    {
        bool MutableSelf { get; }
    }

    public partial interface IShareExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax Referent { get; }
        IBindingMetadata SharedSymbol { get; }
    }

    [Closed(
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
        IMetadata? ReferencedType { get; }
    }

    [Closed(
        typeof(ICapabilityTypeSyntax),
        typeof(IOptionalTypeSyntax),
        typeof(ITypeNameSyntax))]
    public partial interface ITypeSyntax : ISyntax
    {
        DataType? NamedType { get; }
    }

    public partial interface IUnaryOperatorExpressionSyntax : IExpressionSyntax
    {
        UnaryOperatorFixity Fixity { get; }
        UnaryOperator Operator { get; }
        IExpressionSyntax Operand { get; }
    }

    public partial interface IUnsafeExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax Expression { get; }
    }

    public partial interface IUsingDirectiveSyntax : ISyntax
    {
        Name Name { get; }
    }

    public partial interface IVariableDeclarationStatementSyntax : IBodyStatementSyntax, IBindingMetadata
    {
        TextSpan NameSpan { get; }
        SimpleName Name { get; }
        ITypeSyntax? TypeSyntax { get; }
        DataType? Type { get; }
        bool InferMutableType { get; }
        IExpressionSyntax? Initializer { get; }
        bool VariableIsLiveAfter { get; }
    }

    public partial interface IWhileExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax Condition { get; }
        IBlockExpressionSyntax Block { get; }
    }

}
