using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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
        typeof(IBodyOrBlockSyntax),
        typeof(IElseClauseSyntax),
        typeof(IBindingSyntax),
        typeof(IDeclarationSyntax),
        typeof(IParameterSyntax),
        typeof(IReachabilityAnnotationSyntax),
        typeof(IInvocableNameSyntax),
        typeof(IArgumentSyntax),
        typeof(ITypeSyntax),
        typeof(IStatementSyntax),
        typeof(IExpressionSyntax))]
    public partial interface ISyntax
    {
        TextSpan Span { get; }
    }

    public partial interface ICompilationUnitSyntax : ISyntax
    {
        CodeFile File { get; }
        NamespaceName ImplicitNamespaceName { get; }
        FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        FixedList<INonMemberDeclarationSyntax> Declarations { get; }
        FixedList<Diagnostic> Diagnostics { get; }
    }

    public partial interface IUsingDirectiveSyntax : ISyntax
    {
        NamespaceName Name { get; }
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
        typeof(IFieldDeclarationSyntax),
        typeof(IBindingParameterSyntax))]
    public partial interface IBindingSyntax : ISyntax
    {
        bool IsMutableBinding { get; }
        IPromise<BindingSymbol> Symbol { get; }
    }

    [Closed(
        typeof(INamedParameterSyntax),
        typeof(IVariableDeclarationStatementSyntax),
        typeof(IForeachExpressionSyntax))]
    public partial interface ILocalBindingSyntax : IBindingSyntax
    {
        new IPromise<NamedBindingSymbol> Symbol { get; }
    }

    [Closed(
        typeof(IEntityDeclarationSyntax),
        typeof(INonMemberDeclarationSyntax))]
    public partial interface IDeclarationSyntax : ISyntax, IHasContainingLexicalScope
    {
        CodeFile File { get; }
        Name? Name { get; }
        TextSpan NameSpan { get; }
        IPromise<Symbol> Symbol { get; }
    }

    [Closed(
        typeof(IInvocableDeclarationSyntax),
        typeof(INonMemberEntityDeclarationSyntax),
        typeof(IMemberDeclarationSyntax))]
    public partial interface IEntityDeclarationSyntax : IDeclarationSyntax
    {
        IAccessModifierToken? AccessModifier { get; }
    }

    [Closed(
        typeof(IConcreteInvocableDeclarationSyntax),
        typeof(IMethodDeclarationSyntax))]
    public partial interface IInvocableDeclarationSyntax : IEntityDeclarationSyntax
    {
        FixedList<IConstructorParameterSyntax> Parameters { get; }
        FixedList<IReachabilityAnnotationSyntax> ReachabilityAnnotations { get; }
        new IPromise<InvocableSymbol> Symbol { get; }
    }

    [Closed(
        typeof(IFunctionDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax),
        typeof(IConstructorDeclarationSyntax),
        typeof(IAssociatedFunctionDeclarationSyntax))]
    public partial interface IConcreteInvocableDeclarationSyntax : IInvocableDeclarationSyntax
    {
        IBodySyntax Body { get; }
    }

    [Closed(
        typeof(INamespaceDeclarationSyntax),
        typeof(INonMemberEntityDeclarationSyntax))]
    public partial interface INonMemberDeclarationSyntax : IDeclarationSyntax
    {
        NamespaceName ContainingNamespaceName { get; }
        new Name Name { get; }
    }

    public partial interface INamespaceDeclarationSyntax : INonMemberDeclarationSyntax
    {
        bool IsGlobalQualified { get; }
        NamespaceName DeclaredNames { get; }
        NamespaceName FullName { get; }
        new Promise<NamespaceOrPackageSymbol> Symbol { get; }
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
        new AcyclicPromise<ObjectTypeSymbol> Symbol { get; }
        FixedList<IMemberDeclarationSyntax> Members { get; }
        ConstructorSymbol? DefaultConstructorSymbol { get; }
    }

    public partial interface IFunctionDeclarationSyntax : INonMemberEntityDeclarationSyntax, IConcreteInvocableDeclarationSyntax
    {
        new Name Name { get; }
        new FixedList<INamedParameterSyntax> Parameters { get; }
        ITypeSyntax? ReturnType { get; }
        new AcyclicPromise<FunctionSymbol> Symbol { get; }
    }

    [Closed(
        typeof(IMethodDeclarationSyntax),
        typeof(IConstructorDeclarationSyntax),
        typeof(IFieldDeclarationSyntax),
        typeof(IAssociatedFunctionDeclarationSyntax))]
    public partial interface IMemberDeclarationSyntax : IEntityDeclarationSyntax
    {
        IClassDeclarationSyntax DeclaringClass { get; }
    }

    [Closed(
        typeof(IAbstractMethodDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax))]
    public partial interface IMethodDeclarationSyntax : IMemberDeclarationSyntax, IInvocableDeclarationSyntax
    {
        new Name Name { get; }
        ISelfParameterSyntax SelfParameter { get; }
        new FixedList<INamedParameterSyntax> Parameters { get; }
        ITypeSyntax? ReturnType { get; }
        new AcyclicPromise<MethodSymbol> Symbol { get; }
    }

    public partial interface IAbstractMethodDeclarationSyntax : IMethodDeclarationSyntax
    {
    }

    public partial interface IConcreteMethodDeclarationSyntax : IMethodDeclarationSyntax, IConcreteInvocableDeclarationSyntax
    {
        new FixedList<INamedParameterSyntax> Parameters { get; }
        new FixedList<IReachabilityAnnotationSyntax> ReachabilityAnnotations { get; }
    }

    public partial interface IConstructorDeclarationSyntax : IMemberDeclarationSyntax, IConcreteInvocableDeclarationSyntax
    {
        ISelfParameterSyntax ImplicitSelfParameter { get; }
        new AcyclicPromise<ConstructorSymbol> Symbol { get; }
    }

    public partial interface IFieldDeclarationSyntax : IMemberDeclarationSyntax, IBindingSyntax
    {
        new Name Name { get; }
        ITypeSyntax Type { get; }
        new AcyclicPromise<FieldSymbol> Symbol { get; }
        [DisallowNull] ref IExpressionSyntax? Initializer { get; }
    }

    public partial interface IAssociatedFunctionDeclarationSyntax : IMemberDeclarationSyntax, IConcreteInvocableDeclarationSyntax
    {
        new Name Name { get; }
        new FixedList<INamedParameterSyntax> Parameters { get; }
        ITypeSyntax? ReturnType { get; }
        new AcyclicPromise<FunctionSymbol> Symbol { get; }
    }

    [Closed(
        typeof(IConstructorParameterSyntax),
        typeof(IBindingParameterSyntax),
        typeof(INamedParameterSyntax),
        typeof(ISelfParameterSyntax),
        typeof(IFieldParameterSyntax))]
    public partial interface IParameterSyntax : ISyntax
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
    }

    [Closed(
        typeof(INamedParameterSyntax),
        typeof(ISelfParameterSyntax))]
    public partial interface IBindingParameterSyntax : IParameterSyntax, IBindingSyntax
    {
    }

    public partial interface INamedParameterSyntax : IParameterSyntax, IConstructorParameterSyntax, IBindingParameterSyntax, ILocalBindingSyntax
    {
        new Name Name { get; }
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

    public partial interface IInvocableNameSyntax : ISyntax
    {
        Name Name { get; }
    }

    public partial interface IArgumentSyntax : ISyntax
    {
        ref IExpressionSyntax Expression { get; }
    }

    public partial interface IBodySyntax : IBodyOrBlockSyntax
    {
        new FixedList<IBodyStatementSyntax> Statements { get; }
    }

    [Closed(
        typeof(ITypeNameSyntax),
        typeof(IOptionalTypeSyntax),
        typeof(ICapabilityTypeSyntax))]
    public partial interface ITypeSyntax : ISyntax
    {
    }

    public partial interface ITypeNameSyntax : ITypeSyntax, IHasContainingLexicalScope
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
        ref IExpressionSyntax Expression { get; }
    }

    [Closed(
        typeof(IVariableDeclarationStatementSyntax),
        typeof(IExpressionStatementSyntax))]
    public partial interface IBodyStatementSyntax : IStatementSyntax
    {
    }

    public partial interface IVariableDeclarationStatementSyntax : IBodyStatementSyntax, ILocalBindingSyntax
    {
        TextSpan NameSpan { get; }
        Name Name { get; }
        Promise<int?> DeclarationNumber { get; }
        ITypeSyntax? Type { get; }
        bool InferMutableType { get; }
        new Promise<VariableSymbol> Symbol { get; }
        [DisallowNull] ref IExpressionSyntax? Initializer { get; }
    }

    public partial interface IExpressionStatementSyntax : IBodyStatementSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }

    [Closed(
        typeof(IAssignableExpressionSyntax),
        typeof(IBlockExpressionSyntax),
        typeof(INewObjectExpressionSyntax),
        typeof(IUnsafeExpressionSyntax),
        typeof(ILiteralExpressionSyntax),
        typeof(IAssignmentExpressionSyntax),
        typeof(IBinaryOperatorExpressionSyntax),
        typeof(IUnaryOperatorExpressionSyntax),
        typeof(IIfExpressionSyntax),
        typeof(ILoopExpressionSyntax),
        typeof(IWhileExpressionSyntax),
        typeof(IForeachExpressionSyntax),
        typeof(IBreakExpressionSyntax),
        typeof(INextExpressionSyntax),
        typeof(IReturnExpressionSyntax),
        typeof(IImplicitConversionExpressionSyntax),
        typeof(IInvocationExpressionSyntax),
        typeof(ISelfExpressionSyntax),
        typeof(IBorrowExpressionSyntax),
        typeof(IMoveExpressionSyntax),
        typeof(IShareExpressionSyntax))]
    public partial interface IExpressionSyntax : ISyntax
    {
    }

    [Closed(
        typeof(INameExpressionSyntax),
        typeof(IFieldAccessExpressionSyntax))]
    public partial interface IAssignableExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface IBlockExpressionSyntax : IExpressionSyntax, IBlockOrResultSyntax, IBodyOrBlockSyntax
    {
    }

    public partial interface INewObjectExpressionSyntax : IExpressionSyntax
    {
        ITypeNameSyntax Type { get; }
        IInvocableNameSyntax? ConstructorName { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
        Promise<ConstructorSymbol?> ReferencedSymbol { get; }
    }

    public partial interface IUnsafeExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }

    [Closed(
        typeof(IBoolLiteralExpressionSyntax),
        typeof(IIntegerLiteralExpressionSyntax),
        typeof(INoneLiteralExpressionSyntax),
        typeof(IStringLiteralExpressionSyntax))]
    public partial interface ILiteralExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface IBoolLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        bool Value { get; }
    }

    public partial interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        BigInteger Value { get; }
    }

    public partial interface INoneLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
    }

    public partial interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        string Value { get; }
    }

    public partial interface IAssignmentExpressionSyntax : IExpressionSyntax
    {
        ref IAssignableExpressionSyntax LeftOperand { get; }
        AssignmentOperator Operator { get; }
        ref IExpressionSyntax RightOperand { get; }
    }

    public partial interface IBinaryOperatorExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax LeftOperand { get; }
        BinaryOperator Operator { get; }
        ref IExpressionSyntax RightOperand { get; }
    }

    public partial interface IUnaryOperatorExpressionSyntax : IExpressionSyntax
    {
        UnaryOperatorFixity Fixity { get; }
        UnaryOperator Operator { get; }
        ref IExpressionSyntax Operand { get; }
    }

    public partial interface IIfExpressionSyntax : IExpressionSyntax, IElseClauseSyntax
    {
        ref IExpressionSyntax Condition { get; }
        IBlockOrResultSyntax ThenBlock { get; }
        IElseClauseSyntax? ElseClause { get; }
    }

    public partial interface ILoopExpressionSyntax : IExpressionSyntax
    {
        IBlockExpressionSyntax Block { get; }
    }

    public partial interface IWhileExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Condition { get; }
        IBlockExpressionSyntax Block { get; }
    }

    public partial interface IForeachExpressionSyntax : IExpressionSyntax, ILocalBindingSyntax
    {
        Name VariableName { get; }
        Promise<int?> DeclarationNumber { get; }
        ref IExpressionSyntax InExpression { get; }
        ITypeSyntax? Type { get; }
        new Promise<VariableSymbol> Symbol { get; }
        IBlockExpressionSyntax Block { get; }
    }

    public partial interface IBreakExpressionSyntax : IExpressionSyntax
    {
        [DisallowNull] ref IExpressionSyntax? Value { get; }
    }

    public partial interface INextExpressionSyntax : IExpressionSyntax
    {
    }

    public partial interface IReturnExpressionSyntax : IExpressionSyntax
    {
        [DisallowNull] ref IExpressionSyntax? Value { get; }
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
        ObjectType ConvertToType { get; }
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

    [Closed(
        typeof(IFunctionInvocationExpressionSyntax),
        typeof(IMethodInvocationExpressionSyntax))]
    public partial interface IInvocationExpressionSyntax : IExpressionSyntax
    {
        Name Name { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
        IPromise<InvocableSymbol?> ReferencedSymbol { get; }
    }

    public partial interface IFunctionInvocationExpressionSyntax : IInvocationExpressionSyntax, IHasContainingLexicalScope
    {
        NamespaceName Namespace { get; }
        IInvocableNameSyntax FunctionNameSyntax { get; }
        new Promise<FunctionSymbol?> ReferencedSymbol { get; }
    }

    public partial interface IMethodInvocationExpressionSyntax : IInvocationExpressionSyntax, IHasContainingLexicalScope
    {
        ref IExpressionSyntax Context { get; }
        IInvocableNameSyntax MethodNameSyntax { get; }
        new Promise<MethodSymbol?> ReferencedSymbol { get; }
    }

    public partial interface INameExpressionSyntax : IAssignableExpressionSyntax, IHasContainingLexicalScope
    {
        Name? Name { get; }
        Promise<NamedBindingSymbol?> ReferencedSymbol { get; }
    }

    public partial interface ISelfExpressionSyntax : IExpressionSyntax
    {
        bool IsImplicit { get; }
        Promise<SelfParameterSymbol?> ReferencedSymbol { get; }
    }

    public partial interface IFieldAccessExpressionSyntax : IAssignableExpressionSyntax
    {
        ref IExpressionSyntax Context { get; }
        AccessOperator AccessOperator { get; }
        INameExpressionSyntax Field { get; }
        IPromise<FieldSymbol?> ReferencedSymbol { get; }
    }

    public partial interface IBorrowExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Referent { get; }
        Promise<BindingSymbol?> ReferencedSymbol { get; }
    }

    public partial interface IMoveExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Referent { get; }
        Promise<BindingSymbol?> ReferencedSymbol { get; }
    }

    public partial interface IShareExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Referent { get; }
        Promise<BindingSymbol?> ReferencedSymbol { get; }
    }

}
