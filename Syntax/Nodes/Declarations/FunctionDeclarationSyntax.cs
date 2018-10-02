using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        public Token AccessModifier { get; }
        public Token FunctionKeyword { get; }
        public override IdentifierToken Name { get; }
        public Token OpenParen { get; }
        public SeparatedListSyntax<ParameterSyntax> Parameters { get; }
        public Token CloseParen { get; }
        public Token Arrow { get; }
        public TypeSyntax ReturnType { get; }
        public BlockSyntax Body { get; }

        public FunctionDeclarationSyntax(
            Token accessModifier,
            Token functionKeyword,
            IdentifierToken name,
            Token openParen,
            SeparatedListSyntax<ParameterSyntax> parameters,
            Token closeParen,
            Token arrow,
            TypeSyntax returnType,
            BlockSyntax body)
            : base(accessModifier, functionKeyword, name, openParen, parameters, closeParen, arrow, returnType, body)
        {
            AccessModifier = accessModifier;
            FunctionKeyword = functionKeyword;
            Name = name;
            OpenParen = openParen;
            Parameters = parameters;
            CloseParen = closeParen;
            Arrow = arrow;
            ReturnType = returnType;
            Body = body;
        }
    }
}
