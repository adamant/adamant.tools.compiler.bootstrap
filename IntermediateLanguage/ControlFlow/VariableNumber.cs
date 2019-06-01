namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public struct VariableNumber
    {
        public readonly int Value;

        public VariableNumber(int value)
        {
            Value = value;
        }

        public static VariableNumber Result = new VariableNumber(0);

        public override string ToString()
        {
            return Value == 0 ? "result" : Value.ToString();
        }

        public static bool operator ==(VariableNumber a, VariableNumber b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(VariableNumber a, VariableNumber b)
        {
            return a.Value != b.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as VariableNumber?;
            return this == other;
        }
    }
}
