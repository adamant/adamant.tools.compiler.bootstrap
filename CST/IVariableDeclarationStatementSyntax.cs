using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IVariableDeclarationStatementSyntax
    {
        [DisallowNull] ref IExpressionSyntax? Initializer { get; }
        bool VariableIsLiveAfter { get; set; }
    }
}
