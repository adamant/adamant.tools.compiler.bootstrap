using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IBinding),
        typeof(IDeclaration),
        typeof(IParameter),
        typeof(IBody),
        typeof(IExpression))]
    public partial interface IAbstractSyntax
    {
        TextSpan Span { get; }
    }

    [Closed(
        typeof(ILocalBinding),
        typeof(IFieldDeclaration))]
    public partial interface IBinding : IAbstractSyntax
    {
        BindingSymbol Symbol { get; }
    }

    [Closed(
        typeof(IBindingParameter))]
    public partial interface ILocalBinding : IBinding
    {
    }

    [Closed(
        typeof(IExecutableDeclaration),
        typeof(INonMemberDeclaration),
        typeof(IMemberDeclaration))]
    public partial interface IDeclaration : IAbstractSyntax
    {
        Symbol Symbol { get; }
    }

    [Closed(
        typeof(IInvocableDeclaration),
        typeof(IFieldDeclaration))]
    public partial interface IExecutableDeclaration : IDeclaration
    {
    }

    [Closed(
        typeof(IConcreteInvocableDeclaration))]
    public partial interface IInvocableDeclaration : IExecutableDeclaration
    {
        FixedList<IConstructorParameter> Parameters { get; }
    }

    [Closed(
        typeof(IConcreteMethodDeclaration),
        typeof(IConstructorDeclaration),
        typeof(IAssociatedFunctionDeclaration))]
    public partial interface IConcreteInvocableDeclaration : IInvocableDeclaration
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

    public partial interface IFunctionDeclaration : INonMemberDeclaration
    {
        new FunctionSymbol Symbol { get; }
        FixedList<INamedParameter> Parameters { get; }
        IBody Body { get; }
    }

    [Closed(
        typeof(IMethodDeclaration),
        typeof(IConstructorDeclaration),
        typeof(IFieldDeclaration),
        typeof(IAssociatedFunctionDeclaration))]
    public partial interface IMemberDeclaration : IDeclaration
    {
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
    public partial interface IBindingParameter : IParameter, ILocalBinding
    {
    }

    public partial interface INamedParameter : IParameter, IConstructorParameter, IBindingParameter
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

    public partial interface IBody : IAbstractSyntax
    {
    }

    public partial interface IExpression : IAbstractSyntax
    {
    }

}
