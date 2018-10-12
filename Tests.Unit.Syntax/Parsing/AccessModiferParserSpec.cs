using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Parsing
{
    [UnitTest]
    [Category("Parse")]
    public class AccessModiferParserSpec
    {
        [Theory]
        [InlineData("public")]
        [InlineData("protected")]
        [InlineData("private")]
        public void Primitive_types([NotNull] string text)
        {
            var tokens = FakeTokenStream.FromString(text);

            var e = ParseWithoutError(tokens);

            var p = Assert.IsType<AccessModifierSyntax>(e);
            Assert.Equal(tokens[0], p.Keyword);
        }

        [NotNull]
        private static AccessModifierSyntax ParseWithoutError([NotNull] ITokenStream tokenStream)
        {
            var parser = NewAccessModifierParser();
            var diagnostics = new DiagnosticsBuilder();
            var accessModifierSyntax = parser.Parse(tokenStream, diagnostics);
            Assert.Empty(diagnostics.Build());
            return accessModifierSyntax;
        }

        [NotNull]
        private static AccessModifierParser NewAccessModifierParser()
        {
            return new AccessModifierParser();
        }
    }
}
