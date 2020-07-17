namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public sealed class BoolConstantType : BoolType
    {
        internal new static readonly BoolConstantType True = new BoolConstantType(true);
        internal new static readonly BoolConstantType False = new BoolConstantType(false);

        public override bool IsConstant => true;

        public bool Value { get; }

        private BoolConstantType(bool value)
            : base($"const[{(value ? "true" : "false")}]")
        {
            Value = value;
        }

        public override DataType ToNonConstantType()
        {
            return Bool;
        }
    }
}
