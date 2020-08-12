using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IVariableDeclarationStatementSyntax
    {
        // TODO replace this with symbol promise
        [DisallowNull] new DataType? DataType { get; set; }
        [DisallowNull] ref IExpressionSyntax? Initializer { get; }
        bool VariableIsLiveAfter { get; set; }
    }
}
