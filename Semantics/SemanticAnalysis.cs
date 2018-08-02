using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public partial class SemanticAnalysis
    {
        private readonly SemanticAttributes attributes;

        protected SemanticAnalysis(SemanticAttributes attributes)
        {
            this.attributes = attributes;
        }

        public SyntaxBranchNode Parent(SyntaxBranchNode s)
        {
            return attributes.Get<SyntaxBranchNode>(s, "Parent");
        }
    }
}
