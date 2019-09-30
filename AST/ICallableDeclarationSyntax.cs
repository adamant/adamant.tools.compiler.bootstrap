using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Base type for any declaration that declares a callable thing
    /// </summary>
    [Closed(
        typeof(IConstructorDeclarationSyntax),
        typeof(IMethodDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax))]
    public interface ICallableDeclarationSyntax : IEntityDeclarationSyntax, IFunctionSymbol
    {
        new FixedList<IParameterSyntax> Parameters { get; }
        FixedList<IStatementSyntax>? Body { get; }
        [DisallowNull] ControlFlowGraph? ControlFlow { get; set; }
    }
}
