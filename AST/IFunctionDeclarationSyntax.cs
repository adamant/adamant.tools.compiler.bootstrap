using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IFunctionDeclarationSyntax : IMemberDeclarationSyntax,
        ICallableDeclarationSyntax, IFunctionSymbol
    {
        new FixedList<IParameterSyntax> Parameters { get; }
        FixedList<StatementSyntax>? Body { get; }
        [DisallowNull]
        DataType? SelfParameterType { get; set; }
        new TypePromise ReturnType { get; }
        [DisallowNull]
        ControlFlowGraph? ControlFlow { get; set; }

        bool IsExternalFunction { get; set; }
        TypeSyntax? ReturnTypeSyntax { get; }
    }
}
