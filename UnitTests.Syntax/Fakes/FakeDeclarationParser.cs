using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public class FakeDeclarationParser : IParser<DeclarationSyntax>
    {
        public DeclarationSyntax Parse(ITokenStream tokens)
        {
            throw new System.NotImplementedException();
        }
    }
}
