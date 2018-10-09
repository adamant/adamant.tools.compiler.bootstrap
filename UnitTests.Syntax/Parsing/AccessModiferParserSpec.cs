using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Parsing
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

            var e = Parse(tokens);

            var p = Assert.IsType<AccessModifierSyntax>(e);
            Assert.Equal(tokens[0], p.Keyword);
        }

        [NotNull]
        private static AccessModifierSyntax Parse([NotNull] ITokenStream tokenStream)
        {
            var parser = NewAccessModifierParser();
            return parser.Parse(tokenStream);
        }

        [NotNull]
        private static AccessModifierParser NewAccessModifierParser()
        {
            return new AccessModifierParser();
        }
    }
}
