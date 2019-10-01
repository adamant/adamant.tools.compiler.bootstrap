using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class MutableTypeSyntax : TypeSyntax, IMutableTypeSyntax
    {
        public ITypeSyntax Referent { get; }

        public MutableTypeSyntax(TextSpan span, ITypeSyntax referent)
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
