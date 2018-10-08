using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public class FakeUsingDirectiveSyntax : UsingDirectiveSyntax
    {
        public FakeUsingDirectiveSyntax()
        : base(FakeToken.Instance, new FakeNameSyntax(), FakeToken.Instance)
        {
        }
    }
}
