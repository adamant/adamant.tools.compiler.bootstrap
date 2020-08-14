using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class Parameter : AbstractSyntax, IParameter
    {
        public bool Unused { get; }

        protected Parameter(TextSpan span, bool unused)
            : base(span)
        {
            Unused = unused;
        }
    }
}
