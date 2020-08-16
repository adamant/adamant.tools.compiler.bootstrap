using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class Statement : AbstractSyntax, IStatement
    {
        protected Statement(TextSpan span)
            : base(span) { }
    }
}
