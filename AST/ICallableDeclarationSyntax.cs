using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Base type for any declaration that declares a callable thing
    /// </summary>
    [Closed(
        typeof(IConstructorDeclarationSyntax),
        typeof(IMethodDeclarationSyntax))]
    public interface ICallableDeclarationSyntax : IEntityDeclarationSyntax
    {
        FixedList<StatementSyntax>? Body { get; }
    }
}
