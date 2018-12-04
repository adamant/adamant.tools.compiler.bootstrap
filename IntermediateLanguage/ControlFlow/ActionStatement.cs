namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class ActionStatement : ExpressionStatement
    {
        public Value Value { get; }

        public ActionStatement(
            int blockNumber,
            int number,
            Value value)
            : base(blockNumber, number)
        {
            Value = value;
        }

        // Useful for debugging
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
