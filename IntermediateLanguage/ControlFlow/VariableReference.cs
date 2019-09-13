using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class VariableReference : Value, IPlace, IOperand
    {
        public readonly Variable Variable;
        public readonly ValueSemantics ValueSemantics;

        public VariableReference(Variable variable, ValueSemantics valueSemantics, TextSpan span)
            : base(span)
        {
            Variable = variable;
            ValueSemantics = valueSemantics;
        }

        public override string ToString()
        {
            string mutability;
            switch (ValueSemantics)
            {
                case ValueSemantics.LValue:
                    mutability = "";
                    break;
                case ValueSemantics.Empty:
                    mutability = "void ";
                    break;
                case ValueSemantics.Move:
                    mutability = "move ";
                    break;
                case ValueSemantics.Copy:
                    mutability = "copy ";
                    break;
                case ValueSemantics.Own:
                    mutability = "own ";
                    break;
                case ValueSemantics.Borrow:
                    mutability = "borrow ";
                    break;
                case ValueSemantics.Alias:
                    mutability = "alias ";
                    break;
                default:
                    throw ExhaustiveMatch.Failed(ValueSemantics);
            }
            return mutability + Variable;
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
            return ValueSemantics == ValueSemantics.Alias ? this : new VariableReference(Variable, ValueSemantics.Alias, Span);
        }
    }
}
