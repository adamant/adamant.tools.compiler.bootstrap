using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IConstructorDeclarationSyntax : IMemberDeclarationSyntax, ICallableDeclarationSyntax
    {
        FixedList<IParameterSyntax> Parameters { get; }
        new FixedList<StatementSyntax> Body { get; }
        [DisallowNull] DataType? SelfParameterType { get; set; }
        [DisallowNull] ControlFlowGraph? ControlFlow { get; set; }
    }
}
