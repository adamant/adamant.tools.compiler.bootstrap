using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IBodyOrBlock),
        typeof(IElseClause),
        typeof(IBinding),
        typeof(IDeclaration),
        typeof(IParameter),
        typeof(IStatement),
        typeof(IExpression))]
    public partial interface IAbstractSyntax
    {
        TextSpan Span { get; }
    }

    [Closed(
        typeof(IBody),
        typeof(IBlockExpression))]
    public partial interface IBodyOrBlock : IAbstractSyntax
    {
        FixedList<IStatement> Statements { get; }
    }

    [Closed(
        typeof(IBlockOrResult),
        typeof(IIfExpression))]
    public partial interface IElseClause : IAbstractSyntax
    {
    }

    [Closed(
        typeof(IResultStatement),
        typeof(IBlockExpression))]
    public partial interface IBlockOrResult : IElseClause
    {
    }

    [Closed(
        typeof(ILocalBinding),
        typeof(IFieldDeclaration),
        typeof(IBindingParameter))]
    public partial interface IBinding : IAbstractSyntax
    {
        BindingSymbol Symbol { get; }
    }

    [Closed(
        typeof(INamedParameter),
        typeof(IVariableDeclarationStatement),
        typeof(IForeachExpression))]
    public partial interface ILocalBinding : IBinding
    {
        new NamedBindingSymbol Symbol { get; }
    }

    [Closed(
        typeof(IExecutableDeclaration),
        typeof(IInvocableDeclaration),
        typeof(INonMemberDeclaration),
        typeof(IMemberDeclaration))]
    public partial interface IDeclaration : IAbstractSyntax
    {
        CodeFile File { get; }
        Symbol Symbol { get; }
    }

    [Closed(
        typeof(IConcreteInvocableDeclaration),
        typeof(IFieldDeclaration))]
    public partial interface IExecutableDeclaration : IDeclaration
    {
    }

    [Closed(
        typeof(IConcreteInvocableDeclaration))]
    public partial interface IInvocableDeclaration : IDeclaration
    {
        new InvocableSymbol Symbol { get; }
        FixedList<IConstructorParameter> Parameters { get; }
    }

    [Closed(
        typeof(IFunctionDeclaration),
        typeof(IConcreteMethodDeclaration),
        typeof(IConstructorDeclaration),
        typeof(IAssociatedFunctionDeclaration))]
    public partial interface IConcreteInvocableDeclaration : IInvocableDeclaration, IExecutableDeclaration
    {
        IBody Body { get; }
    }

    [Closed(
        typeof(IClassDeclaration),
        typeof(IFunctionDeclaration))]
    public partial interface INonMemberDeclaration : IDeclaration
    {
    }

    public partial interface IClassDeclaration : INonMemberDeclaration
    {
        new ObjectTypeSymbol Symbol { get; }
        FixedList<IMemberDeclaration> Members { get; }
    }

    public partial interface IFunctionDeclaration : INonMemberDeclaration, IConcreteInvocableDeclaration
    {
        new FunctionSymbol Symbol { get; }
        new FixedList<INamedParameter> Parameters { get; }
    }

    [Closed(
        typeof(IMethodDeclaration),
        typeof(IConstructorDeclaration),
        typeof(IFieldDeclaration),
        typeof(IAssociatedFunctionDeclaration))]
    public partial interface IMemberDeclaration : IDeclaration
    {
        IClassDeclaration DeclaringClass { get; }
    }

    [Closed(
        typeof(IAbstractMethodDeclaration),
        typeof(IConcreteMethodDeclaration))]
    public partial interface IMethodDeclaration : IMemberDeclaration
    {
        new MethodSymbol Symbol { get; }
        ISelfParameter SelfParameter { get; }
        FixedList<INamedParameter> Parameters { get; }
    }

    public partial interface IAbstractMethodDeclaration : IMethodDeclaration
    {
    }

    public partial interface IConcreteMethodDeclaration : IMethodDeclaration, IConcreteInvocableDeclaration
    {
        new FixedList<INamedParameter> Parameters { get; }
    }

    public partial interface IConstructorDeclaration : IMemberDeclaration, IConcreteInvocableDeclaration
    {
        new ConstructorSymbol Symbol { get; }
        ISelfParameter ImplicitSelfParameter { get; }
    }

    public partial interface IFieldDeclaration : IMemberDeclaration, IExecutableDeclaration, IBinding
    {
        new FieldSymbol Symbol { get; }
    }

    public partial interface IAssociatedFunctionDeclaration : IMemberDeclaration, IConcreteInvocableDeclaration
    {
        new FunctionSymbol Symbol { get; }
        new FixedList<INamedParameter> Parameters { get; }
    }

    [Closed(
        typeof(IConstructorParameter),
        typeof(IBindingParameter),
        typeof(INamedParameter),
        typeof(ISelfParameter),
        typeof(IFieldParameter))]
    public partial interface IParameter : IAbstractSyntax
    {
        bool Unused { get; }
    }

    [Closed(
        typeof(INamedParameter),
        typeof(IFieldParameter))]
    public partial interface IConstructorParameter : IParameter
    {
    }

    [Closed(
        typeof(INamedParameter),
        typeof(ISelfParameter))]
    public partial interface IBindingParameter : IParameter, IBinding
    {
    }

    public partial interface INamedParameter : IParameter, IConstructorParameter, IBindingParameter, ILocalBinding
    {
        new VariableSymbol Symbol { get; }
        IExpression? DefaultValue { get; }
    }

    public partial interface ISelfParameter : IParameter, IBindingParameter
    {
        new SelfParameterSymbol Symbol { get; }
    }

    public partial interface IFieldParameter : IParameter, IConstructorParameter
    {
        FieldSymbol ReferencedSymbol { get; }
        IExpression? DefaultValue { get; }
    }

    public partial interface IBody : IBodyOrBlock
    {
        new FixedList<IBodyStatement> Statements { get; }
    }

    [Closed(
        typeof(IResultStatement),
        typeof(IBodyStatement))]
    public partial interface IStatement : IAbstractSyntax
    {
    }

    public partial interface IResultStatement : IStatement, IBlockOrResult
    {
        IExpression Expression { get; }
    }

    [Closed(
        typeof(IVariableDeclarationStatement),
        typeof(IExpressionStatement))]
    public partial interface IBodyStatement : IStatement
    {
    }

    public partial interface IVariableDeclarationStatement : IBodyStatement, ILocalBinding
    {
        TextSpan NameSpan { get; }
        new VariableSymbol Symbol { get; }
        IExpression? Initializer { get; }
        Promise<bool> VariableIsLiveAfter { get; }
    }

    public partial interface IExpressionStatement : IBodyStatement
    {
        IExpression Expression { get; }
    }

    [Closed(
        typeof(IAssignableExpression),
        typeof(IBlockExpression),
        typeof(INewObjectExpression),
        typeof(IUnsafeExpression),
        typeof(ILiteralExpression),
        typeof(IAssignmentExpression),
        typeof(IBinaryOperatorExpression),
        typeof(IUnaryOperatorExpression),
        typeof(IIfExpression),
        typeof(ILoopExpression),
        typeof(IWhileExpression),
        typeof(IForeachExpression),
        typeof(IBreakExpression),
        typeof(INextExpression),
        typeof(IReturnExpression),
        typeof(IImplicitConversionExpression),
        typeof(IInvocationExpression),
        typeof(ISelfExpression),
        typeof(IBorrowExpression),
        typeof(IMoveExpression),
        typeof(IShareExpression))]
    public partial interface IExpression : IAbstractSyntax
    {
        DataType DataType { get; }
        ExpressionSemantics Semantics { get; }
    }

    [Closed(
        typeof(INameExpression),
        typeof(IFieldAccessExpression))]
    public partial interface IAssignableExpression : IExpression
    {
    }

    public partial interface IBlockExpression : IExpression, IBlockOrResult, IBodyOrBlock
    {
    }

    public partial interface INewObjectExpression : IExpression
    {
        ConstructorSymbol ReferencedSymbol { get; }
        FixedList<IExpression> Arguments { get; }
    }

    public partial interface IUnsafeExpression : IExpression
    {
        IExpression Expression { get; }
    }

    [Closed(
        typeof(IBoolLiteralExpression),
        typeof(IIntegerLiteralExpression),
        typeof(INoneLiteralExpression),
        typeof(IStringLiteralExpression))]
    public partial interface ILiteralExpression : IExpression
    {
    }

    public partial interface IBoolLiteralExpression : ILiteralExpression
    {
        bool Value { get; }
    }

    public partial interface IIntegerLiteralExpression : ILiteralExpression
    {
        BigInteger Value { get; }
    }

    public partial interface INoneLiteralExpression : ILiteralExpression
    {
    }

    public partial interface IStringLiteralExpression : ILiteralExpression
    {
        string Value { get; }
    }

    public partial interface IAssignmentExpression : IExpression
    {
        IAssignableExpression LeftOperand { get; }
        AssignmentOperator Operator { get; }
        IExpression RightOperand { get; }
    }

    public partial interface IBinaryOperatorExpression : IExpression
    {
        IExpression LeftOperand { get; }
        BinaryOperator Operator { get; }
        IExpression RightOperand { get; }
    }

    public partial interface IUnaryOperatorExpression : IExpression
    {
        UnaryOperatorFixity Fixity { get; }
        UnaryOperator Operator { get; }
        IExpression Operand { get; }
    }

    public partial interface IIfExpression : IExpression, IElseClause
    {
        IExpression Condition { get; }
        IBlockOrResult ThenBlock { get; }
        IElseClause? ElseClause { get; }
    }

    public partial interface ILoopExpression : IExpression
    {
        IBlockExpression Block { get; }
    }

    public partial interface IWhileExpression : IExpression
    {
        IExpression Condition { get; }
        IBlockExpression Block { get; }
    }

    public partial interface IForeachExpression : IExpression, ILocalBinding
    {
        new VariableSymbol Symbol { get; }
        IExpression InExpression { get; }
        IBlockExpression Block { get; }
        Promise<bool> VariableIsLiveAfterAssignment { get; }
    }

    public partial interface IBreakExpression : IExpression
    {
        IExpression? Value { get; }
    }

    public partial interface INextExpression : IExpression
    {
    }

    public partial interface IReturnExpression : IExpression
    {
        IExpression? Value { get; }
    }

    [Closed(
        typeof(IImplicitImmutabilityConversionExpression),
        typeof(IImplicitNoneConversionExpression),
        typeof(IImplicitNumericConversionExpression),
        typeof(IImplicitOptionalConversionExpression))]
    public partial interface IImplicitConversionExpression : IExpression
    {
        IExpression Expression { get; }
    }

    public partial interface IImplicitImmutabilityConversionExpression : IImplicitConversionExpression
    {
        ObjectType ConvertToType { get; }
    }

    public partial interface IImplicitNoneConversionExpression : IImplicitConversionExpression
    {
        OptionalType ConvertToType { get; }
    }

    public partial interface IImplicitNumericConversionExpression : IImplicitConversionExpression
    {
        NumericType ConvertToType { get; }
    }

    public partial interface IImplicitOptionalConversionExpression : IImplicitConversionExpression
    {
        OptionalType ConvertToType { get; }
    }

    [Closed(
        typeof(IFunctionInvocationExpression),
        typeof(IMethodInvocationExpression))]
    public partial interface IInvocationExpression : IExpression
    {
        FixedList<IExpression> Arguments { get; }
    }

    public partial interface IFunctionInvocationExpression : IInvocationExpression
    {
        FunctionSymbol ReferencedSymbol { get; }
    }

    public partial interface IMethodInvocationExpression : IInvocationExpression
    {
        IExpression Context { get; }
        MethodSymbol ReferencedSymbol { get; }
    }

    public partial interface INameExpression : IAssignableExpression
    {
        NamedBindingSymbol ReferencedSymbol { get; }
        Promise<bool> VariableIsLiveAfter { get; }
    }

    public partial interface ISelfExpression : IExpression
    {
        SelfParameterSymbol ReferencedSymbol { get; }
        bool IsImplicit { get; }
    }

    public partial interface IFieldAccessExpression : IAssignableExpression
    {
        IExpression Context { get; }
        AccessOperator AccessOperator { get; }
        FieldSymbol ReferencedSymbol { get; }
    }

    public partial interface IBorrowExpression : IExpression
    {
        BindingSymbol ReferencedSymbol { get; }
        IExpression Referent { get; }
    }

    public partial interface IMoveExpression : IExpression
    {
        BindingSymbol ReferencedSymbol { get; }
        IExpression Referent { get; }
    }

    public partial interface IShareExpression : IExpression
    {
        BindingSymbol ReferencedSymbol { get; }
        IExpression Referent { get; }
    }

}
