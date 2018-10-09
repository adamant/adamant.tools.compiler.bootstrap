using System;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public class FakeQualifiedNameParser : IParser<NameSyntax>
    {
        public NameSyntax Parse([NotNull] ITokenStream tokens)
        {
            var fakeToken = tokens.ExpectFake();
            return (NameSyntax)fakeToken?.FakeNode ?? throw new InvalidOperationException();
        }
    }
}
