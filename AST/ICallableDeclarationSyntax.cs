using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Base type for any declaration that declares a callable thing
    /// </summary>
    [Closed(
        typeof(IMethodDeclarationSyntax),
        typeof(IConcreteCallableDeclarationSyntax))]
    public interface ICallableDeclarationSyntax : IEntityDeclarationSyntax, IFunctionSymbol
    {
        new FixedList<IParameterSyntax> Parameters { get; }
        FixedList<IReachabilityAnnotationSyntax> ReachabilityAnnotations { get; }
    }
}
