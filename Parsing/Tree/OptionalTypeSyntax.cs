using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    class OptionalTypeSyntax : TypeSyntax, IOptionalTypeSyntax
    {
        public ITypeSyntax Referent { get; }

        public OptionalTypeSyntax(TextSpan span, ITypeSyntax referent)
            : base(span)
        {
            Referent = referent;
        }

        public override string ToString()
        {
            return $"{Referent}?";
        }
    }
}
