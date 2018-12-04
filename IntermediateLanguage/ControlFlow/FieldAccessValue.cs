using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class FieldAccessValue : Value
    {
        public Operand Expression { get; }
        public Name Field { get; }

        public FieldAccessValue(Operand expression, Name field)
        {
            Expression = expression;
            Field = field;
        }

        public override string ToString()
        {
            return $"{Expression}.{Field}";
        }
    }
}
