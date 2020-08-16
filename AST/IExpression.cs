using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public partial interface IExpression
    {
        string ToGroupedString(OperatorPrecedence surroundingPrecedence);
    }
}
