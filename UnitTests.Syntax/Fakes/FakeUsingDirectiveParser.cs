using System;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Helpers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public class FakeUsingDirectiveParser : IParser<UsingDirectiveSyntax>
    {
        [NotNull]
        public UsingDirectiveSyntax Parse([NotNull] ITokenStream tokens)
        {
            var _ = tokens.Expect<UsingKeywordToken>();

            var fakeToken = tokens.ExpectFake();
            return (UsingDirectiveSyntax)fakeToken?.FakeNode ?? throw new InvalidOperationException();
        }
    }
}
