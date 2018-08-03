using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        public Token AccessModifier { get; }
        public override IdentifierToken Name { get; }
        public ParameterListSyntax ParameterList { get; }
        public IReadOnlyList<ParameterSyntax> Parameters { get; }
        public TypeSyntax ReturnType { get; }
        public BlockSyntax Body { get; }

        public FunctionDeclarationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            AccessModifier = (Token)Children.First();
            Name = Children.OfType<IdentifierToken>().Single();
            ParameterList = Children.OfType<ParameterListSyntax>().Single();
            Parameters = ParameterList.Parameters;
            ReturnType = Children.OfType<TypeSyntax>().Single();
            Body = Children.OfType<BlockSyntax>().Single();
        }
    }
}
