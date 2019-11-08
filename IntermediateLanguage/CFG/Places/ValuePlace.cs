using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places
{
    public abstract class ValuePlace : Place
    {
        protected ValuePlace(TextSpan span)
            : base(span) { }

        public Operand Reference()
        {
            throw new System.NotImplementedException();
        }
    }
}
