using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class TypeName : SemanticNode
    {
        public new TypeSyntax Syntax { get; }
        public DataType Type { get; }

        public TypeName(TypeSyntax syntax, DataType type)
        {
            Syntax = syntax;
            Type = type;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
