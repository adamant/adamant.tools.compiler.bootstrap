using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    [Closed(
        typeof(FunctionDeclaration),
        typeof(ConstructorDeclaration))]
    public interface ICallableDeclaration
    {
        ControlFlowGraph? IL { get; }
    }
}
