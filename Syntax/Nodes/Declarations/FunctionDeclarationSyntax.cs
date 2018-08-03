using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class FunctionDeclarationSyntax : DeclarationSyntax
    {
        public Token AccessModifier => (Token)Children.First();
        public override IdentifierToken Name => Children.OfType<IdentifierToken>().Single();
        public ParameterListSyntax ParameterList => Children.OfType<ParameterListSyntax>().Single();
        public IEnumerable<ParameterSyntax> Parameters => ParameterList.Parameters;
        public TypeSyntax ReturnType => Children.OfType<TypeSyntax>().Single();
        public BlockSyntax Body => Children.OfType<BlockSyntax>().Single();

        public FunctionDeclarationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
