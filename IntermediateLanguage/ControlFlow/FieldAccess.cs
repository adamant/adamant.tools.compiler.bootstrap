using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class FieldAccess : Value, IPlace
    {
        public IOperand Expression { get; }
        public Name Field { get; }

        public FieldAccess(IOperand expression, Name field, TextSpan span)
            : base(span)
        {
            Expression = expression;
            Field = field;
        }

        public Variable CoreVariable()
        {
            if (Expression is IPlace place) return place.CoreVariable();
            return new Variable(-1); // There is no variable
        }

        public override string ToString()
        {
            return $"({Expression}).{Field.UnqualifiedName}";
        }
    }
}
