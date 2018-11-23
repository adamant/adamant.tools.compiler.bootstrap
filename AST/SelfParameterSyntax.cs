using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class SelfParameterSyntax : ParameterSyntax
    {
        public bool RefSelf { get; }
        public bool MutableSelf { get; }

        public SelfParameterSyntax(TextSpan span, bool refSelf, bool mutableSelf)
            : base(span, false, SpecialName.Self)
        {
            RefSelf = refSelf;
            MutableSelf = mutableSelf;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
