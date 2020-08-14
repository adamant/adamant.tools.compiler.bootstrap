using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    [Closed(
        typeof(FunctionDeclaration),
        typeof(MethodDeclaration),
        typeof(ConstructorDeclaration))]
    public interface IInvocableDeclaration
    {
        bool IsConstructor { get; }
        FixedList<Parameter> Parameters { get; }
        ControlFlowGraph? IL { get; }
    }
}
