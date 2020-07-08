using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands
{
    public class VariableReference : Operand
    {
        public Variable Variable { get; }
        public OldValueSemantics ValueSemantics { get; }

        public VariableReference(Variable variable, OldValueSemantics valueSemantics, TextSpan span)
            : base(span)
        {
            Variable = variable;
            ValueSemantics = valueSemantics;
        }

        public override string ToString()
        {
            return ValueSemantics.Action() + " " + Variable;
        }

        public VariableReference AsOwn(TextSpan span)
        {
            return new VariableReference(Variable, OldValueSemantics.Own, span);
        }

        public VariableReference AsBorrow()
        {
            return ValueSemantics == OldValueSemantics.Borrow ? this : new VariableReference(Variable, OldValueSemantics.Borrow, Span);
        }

        public VariableReference AsAlias()
        {
            return ValueSemantics == OldValueSemantics.Share ? this : new VariableReference(Variable, OldValueSemantics.Share, Span);
        }
    }
}
