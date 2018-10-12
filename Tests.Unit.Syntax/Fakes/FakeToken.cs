using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes
{
    public class FakeToken : Token
    {
        [CanBeNull]
        public readonly SyntaxNode FakeNode;

        public FakeToken(TextSpan span, [CanBeNull] SyntaxNode fakeNode)
            : base(span)
        {
            FakeNode = fakeNode;
        }
    }
}
