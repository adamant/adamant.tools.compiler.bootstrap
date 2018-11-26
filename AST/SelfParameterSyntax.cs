using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class SelfParameterSyntax : ParameterSyntax
    {
        public bool RefSelf { get; }
        public bool MutableSelf { get; }

        public SelfParameterSyntax(TextSpan span, [NotNull] Name fullName, bool refSelf, bool mutableSelf)
            : base(span, false, fullName)
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
