using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public abstract class SemanticNode
    {
        public SyntaxNode Syntax => GetSyntax();

        protected abstract SyntaxNode GetSyntax();
    }
}
