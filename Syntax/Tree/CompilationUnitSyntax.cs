using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tree
{
    public class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(IEnumerable<Syntax> children)
            : base(children)
        {
        }
    }
}
