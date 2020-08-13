using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    [Closed(
        typeof(FunctionDeclaration),
        typeof(MethodDeclaration),
        typeof(ConstructorDeclaration))]
    public interface IInvocableDeclaration
    {
        bool IsExternal { get; }
        bool IsConstructor { get; }
        FixedList<Parameter> Parameters { get; }
        int Arity { get; }
        DataType ReturnDataType { get; }
        ControlFlowGraph? IL { get; }
    }
}