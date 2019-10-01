using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IMethodDeclarationSyntax : IMemberDeclarationSyntax,
        ICallableDeclarationSyntax
    {
        // TODO remove, there is no such thing as an external method
        bool IsExternalFunction { get; set; }
        new FixedList<IParameterSyntax> Parameters { get; }
        FixedList<IStatementSyntax>? Body { get; }
        [DisallowNull] DataType? SelfParameterType { get; set; }
        new TypePromise ReturnType { get; }
        ITypeSyntax? ReturnTypeSyntax { get; }
        [DisallowNull] ControlFlowGraph? ControlFlow { get; set; }
    }
}
