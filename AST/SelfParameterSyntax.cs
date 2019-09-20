using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class SelfParameterSyntax : ParameterSyntax
    {
        public bool MutableSelf { get; }

        public SelfParameterSyntax(TextSpan span, Name fullName, bool mutableSelf)
            : base(span, false, fullName)
        {
            MutableSelf = mutableSelf;
        }

        public override string ToString()
        {
            var value = "self";
            if (MutableSelf)
                value = "mut " + value;
            return value;
        }
    }
}
