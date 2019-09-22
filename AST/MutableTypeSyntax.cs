using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class MutableTypeSyntax : TypeSyntax
    {
        public TypeSyntax Referent { get; }

        public MutableTypeSyntax(TextSpan span, TypeSyntax referent)
            : base(span)
        {
            Referent = referent;
        }

        public override string ToString()
        {
            return $"mut {Referent}";
        }
    }
}
