using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Parsing
{
    [UnitTest]
    [Category("Parse")]
    public class ModiferParserSpec
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
            Assert.Equal(tokens[0], p.Token);
        }

        [NotNull]
        private static ModifierSyntax ParseWithoutError([NotNull] ITokenStream tokenStream)
        {
            var parser = NewAccessModifierParser();
            var diagnostics = new DiagnosticsBuilder();
            var modifierSyntax = parser.AcceptModifier(tokenStream, diagnostics);
            Assert.Empty(diagnostics.Build());
            return modifierSyntax;
        }

        [NotNull]
        private static ModifierParser NewAccessModifierParser()
        {
            return new ModifierParser();
        }
    }
}
