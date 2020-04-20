using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class VariableReference : Value, IPlace, IOperand
    {
        public Variable Variable { get; }
        public ValueSemantics ValueSemantics { get; }

        public VariableReference(Variable variable, ValueSemantics valueSemantics, TextSpan span)
            : base(span)
        {
            Variable = variable;
            ValueSemantics = valueSemantics;
        }

        public override string ToString()
        {
            return ValueSemantics.Action() + " " + Variable;
        }

        public Variable CoreVariable()
        {
            return Variable;
        }

        public VariableReference AsOwn(TextSpan span)
        {
            return new VariableReference(Variable, ValueSemantics.Own, span);
        }

        public VariableReference AsBorrow()
        {
            return ValueSemantics == ValueSemantics.Borrow ? this : new VariableReference(Variable, ValueSemantics.Borrow, Span);
        }

        public VariableReference AsAlias()
        {
            return ValueSemantics == ValueSemantics.Share ? this : new VariableReference(Variable, ValueSemantics.Share, Span);
        }
    }
}
