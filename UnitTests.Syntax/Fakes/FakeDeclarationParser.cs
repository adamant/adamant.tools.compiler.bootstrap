using System;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Helpers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public class FakeDeclarationParser : IParser<DeclarationSyntax>
    {
        [NotNull]
        public DeclarationSyntax Parse([NotNull] ITokenStream tokens)
        {
            var fakeToken = tokens.ExpectFake();
            return (DeclarationSyntax)fakeToken?.FakeNode ?? throw new InvalidOperationException();
        }
    }
}
