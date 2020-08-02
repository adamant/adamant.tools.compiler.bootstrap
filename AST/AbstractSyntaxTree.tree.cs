using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IDeclaration))]
    public partial interface IAbstractSyntax
    {
        TextSpan TextSpan { get; }
    }

    [Closed(
        typeof(IMemberDeclaration),
        typeof(INonMemberEntityDeclaration))]
    public partial interface IDeclaration : IAbstractSyntax
    {
        Symbol Symbol { get; }
    }

    public partial interface IMemberDeclaration : IDeclaration
    {
    }

    [Closed(
        typeof(IClassDeclaration))]
    public partial interface INonMemberEntityDeclaration : IDeclaration
    {
    }

    public partial interface IClassDeclaration : INonMemberEntityDeclaration
    {
    }

}
