using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class SelfParameterSyntax : ParameterSyntax, ISelfParameterSyntax
    {
        public override bool IsMutableBinding => MutableSelf;
        public bool MutableSelf { get; }

        public SelfParameterSyntax(TextSpan span, Name fullName, bool mutableSelf)
            : base(span, fullName)
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
