using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class VariableReference : Place
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
                case ValueSemantics.Borrow:
                    mutability = "borrow ";
                    break;
                case ValueSemantics.Alias:
                    mutability = "alias ";
                    break;
                default:
                    throw NonExhaustiveMatchException.ForEnum(ValueSemantics);
            }
            return mutability + Variable;
        }

        public override Variable CoreVariable()
        {
            return Variable;
        }

        public Value AsMove(TextSpan span)
        {
            return new VariableReference(Variable, ValueSemantics.Move, span);
        }

        public Value AsBorrow()
        {
            return ValueSemantics == ValueSemantics.Borrow ? this : new VariableReference(Variable, ValueSemantics.Borrow, Span);
        }

        public Value AsAlias()
        {
            return ValueSemantics == ValueSemantics.Alias ? this : new VariableReference(Variable, ValueSemantics.Alias, Span);
        }
    }
}
