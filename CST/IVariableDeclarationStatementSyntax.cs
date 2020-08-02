using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IVariableDeclarationStatementSyntax
    {
        [DisallowNull] new DataType? DataType { get; set; }
        [DisallowNull] ref IExpressionSyntax? Initializer { get; }
        bool VariableIsLiveAfter { get; set; }
    }
}
