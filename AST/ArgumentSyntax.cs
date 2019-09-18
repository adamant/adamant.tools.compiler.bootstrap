namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ArgumentSyntax : Syntax
    {
        //public bool IsParams { get; }
        public ExpressionSyntax Value;

        public ArgumentSyntax(
            //bool isParams,
            ExpressionSyntax value)
        {
            //IsParams = isParams;
            Value = value;
        }

        public override string ToString()
        {
            //if (IsParams)
            //    return $"params {Value}";
            return Value?.ToString() ?? "";
        }
    }
}
