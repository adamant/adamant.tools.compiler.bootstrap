using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// Base type for any declaration that declares a callable thing
    /// </summary>
    [Closed(
        typeof(IMethodDeclarationSyntax),
        typeof(IConcreteCallableDeclarationSyntax))]
    public interface ICallableDeclarationSyntax : IEntityDeclarationSyntax, IFunctionMetadata
    {
        new FixedList<IParameterSyntax> Parameters { get; }
        FixedList<IReachabilityAnnotationSyntax> ReachabilityAnnotations { get; }
    }
}
