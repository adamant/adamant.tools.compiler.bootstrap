namespace Adamant.Tools.Compiler.Bootstrap.Core.Syntax
{
    public static class Operators
    {
        public static bool IsOperator(this TokenKind tokenKind)
        {
            return tokenKind >= TokenKind.FirstOperator && tokenKind <= TokenKind.LastOperator;
        }
    }
}
