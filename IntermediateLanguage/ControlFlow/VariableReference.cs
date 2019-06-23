using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class VariableReference : Place
    {
        public readonly Variable Variable;
        public readonly VariableReferenceKind Kind;

        public VariableReference(Variable variable, VariableReferenceKind kind, TextSpan span)
            : base(span)
        {
            Variable = variable;
            Kind = kind;
        }

        public override string ToString()
        {
            string mutability;
            switch (Kind)
            {
                case VariableReferenceKind.Assign:
                    mutability = "";
                    break;
                case VariableReferenceKind.Borrow:
                    mutability = "mut ";
                    break;
                case VariableReferenceKind.Alias:
                    mutability = "imm ";
                    break;
                case VariableReferenceKind.Move:
                    mutability = "move ";
                    break;
                default:
                    throw NonExhaustiveMatchException.ForEnum(Kind);
            }
            return mutability + Variable;
        }

        public override Variable CoreVariable()
        {
            return Variable;
        }

        public Value AsShared()
        {
            return Kind == VariableReferenceKind.Alias ? this : new VariableReference(Variable, VariableReferenceKind.Alias, Span);
        }

        public Value AsMove(TextSpan span)
        {
            return new VariableReference(Variable, VariableReferenceKind.Move, span);

        }
    }
}
