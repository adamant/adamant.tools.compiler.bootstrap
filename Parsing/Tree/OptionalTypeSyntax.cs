using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

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
