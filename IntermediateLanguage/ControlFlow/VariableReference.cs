using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class VariableReference : Place
    {
        public readonly VariableNumber VariableNumber;
        public readonly VariableReferenceKind Kind;

        public VariableReference(VariableNumber variableNumber, VariableReferenceKind kind, TextSpan span)
            : base(span)
        {
            VariableNumber = variableNumber;
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
                case VariableReferenceKind.Share:
                    mutability = "imm ";
                    break;
                default:
                    throw NonExhaustiveMatchException.ForEnum(Kind);
            }
            return $"{mutability}%{VariableNumber}";
        }

        public override VariableNumber CoreVariable()
        {
            return VariableNumber;
        }

        public Value AsShared()
        {
            return Kind == VariableReferenceKind.Share ? this : new VariableReference(VariableNumber, VariableReferenceKind.Share, Span);
        }
    }
}
