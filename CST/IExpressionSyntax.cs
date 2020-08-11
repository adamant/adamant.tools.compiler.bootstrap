using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IExpressionSyntax
    {
        [DisallowNull] DataType? DataType { get; set; }
        [DisallowNull] ExpressionSemantics? Semantics { get; set; }
        string ToGroupedString(OperatorPrecedence surroundingPrecedence);
    }
}
