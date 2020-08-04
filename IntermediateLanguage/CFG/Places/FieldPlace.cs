using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places
{
    public class FieldPlace : Place
    {
        public Operand Target { get; }
        public MaybeQualifiedName Field { get; }

        public FieldPlace(Operand target, MaybeQualifiedName field, TextSpan span)
            : base(span)
        {
            Target = target;
            Field = field;
        }

        public override Operand ToOperand(TextSpan span)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"({Target}).{Field.UnqualifiedName}";
        }
    }
}
