using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

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
