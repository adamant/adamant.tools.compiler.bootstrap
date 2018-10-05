using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ParameterSyntax : SyntaxNode
    {
        public SimpleToken? VarKeyword { get; }
        public IdentifierToken Name { get; }
        public SimpleToken Colon { get; }
        public TypeSyntax Type { get; }

        public ParameterSyntax(SimpleToken? varKeyword, IdentifierToken name, SimpleToken colon, TypeSyntax type)
        {
            VarKeyword = varKeyword;
            Name = name;
            Colon = colon;
            Type = type;
        }
    }
}
