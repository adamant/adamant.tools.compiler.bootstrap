using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    [Closed(
        typeof(FunctionDeclaration),
        typeof(ConstructorDeclaration))]
    public interface ICallableDeclaration
    {
        ControlFlowGraph? ControlFlow { get; }
    }
}
