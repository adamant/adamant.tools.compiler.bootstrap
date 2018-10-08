using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public class FakeQualifiedNameParser : IParser<NameSyntax>
    {
        public NameSyntax Parse(ITokenStream tokens)
        {
            var fakeToken = tokens.ExpectFake();
            return (NameSyntax)fakeToken.Value;
        }
    }
}
