using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class FunctionDeclarationSyntax : DeclarationSyntax
    {
        public Token AccessModifier => (Token)Children.First();
        public IdentifierToken Name => Children.OfType<IdentifierToken>().Single();
        public ParameterListSyntax Parameters => Children.OfType<ParameterListSyntax>().Single();
        public TypeSyntax ReturnType => Children.OfType<TypeSyntax>().Single();
        public BlockSyntax Body => Children.OfType<BlockSyntax>().Single();

        public FunctionDeclarationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
