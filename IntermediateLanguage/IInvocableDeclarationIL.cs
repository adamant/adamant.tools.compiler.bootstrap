using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    [Closed(
        typeof(FunctionIL),
        typeof(MethodDeclarationIL),
        typeof(ConstructorIL))]
    public interface IInvocableDeclarationIL
    {
        bool IsConstructor { get; }
        FixedList<ParameterIL> Parameters { get; }
        ControlFlowGraph? IL { get; }
    }
}
