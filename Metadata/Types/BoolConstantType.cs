namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class BoolConstantType : BoolType
    {
        internal new static readonly BoolConstantType True = new BoolConstantType(true);
        internal new static readonly BoolConstantType False = new BoolConstantType(false);

        public bool Value { get; }

        private BoolConstantType(bool value)
            : base($"const[{(value ? "true" : "false")}]")
        {
            Value = value;
        }
    }
}
