using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ParameterSyntax : SyntaxBranchNode
    {
        public Token VarKeyword { get; }
        public IdentifierToken Name { get; }
        public Token Colon { get; }
        public TypeSyntax Type { get; }

        public ParameterSyntax(Token varKeyword, IdentifierToken name, Token colon, TypeSyntax type)
            : base(varKeyword, name, colon, type)
        {
            VarKeyword = varKeyword;
            Name = name;
            Colon = colon;
            Type = type;
        }
    }
}
