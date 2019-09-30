using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(INamedFunctionDeclarationSyntax))]
    public interface IFunctionDeclarationSyntax : IMemberDeclarationSyntax, IFunctionSymbol
    {
        new FixedList<ParameterSyntax> Parameters { get; }
        FixedList<StatementSyntax>? Body { get; }
        [DisallowNull]
        DataType? SelfParameterType { get; set; }
        new TypePromise ReturnType { get; }
        [DisallowNull]
        ControlFlowGraph? ControlFlow { get; set; }
    }
}
