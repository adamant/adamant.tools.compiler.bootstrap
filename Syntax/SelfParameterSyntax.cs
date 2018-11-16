using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SelfParameterSyntax : ParameterSyntax
    {
        public bool IsRef { get; }
        public bool MutableBinding { get; }

        public SelfParameterSyntax(TextSpan span, bool isRef, bool mutableBinding)
            : base(span)
        {
            IsRef = isRef;
            MutableBinding = mutableBinding;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
