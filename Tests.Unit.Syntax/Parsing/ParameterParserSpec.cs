using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Parsing
{
    [UnitTest]
    [Category("Parse")]
    public class ParameterParserSpec
    {
        [Fact]
        public void Unused_parameter()
        {
            var type = FakeSyntax.Name();
            var tokens = FakeTokenStream.From($"_: {type};");

            var p = ParseWithoutError(tokens);

            var np = Assert.IsType<NamedParameterSyntax>(p);
            Assert.Null(np.VarKeyword);
            Assert.Equal(tokens[0], np.Name);
            Assert.Equal(tokens[1], np.Colon);
            Assert.Equal(type, np.TypeExpression);
        }

        [NotNull]
        private static ParameterSyntax ParseWithoutError([NotNull] ITokenStream tokens)
        {
            var parser = NewAccessModifierParser();
            var diagnostics = new DiagnosticsBuilder();
            var parameterSyntax = parser.ParseParameter(tokens, diagnostics);
            Assert.Empty(diagnostics.Build());
            return parameterSyntax;
        }

        [NotNull]
        private static ParameterParser NewAccessModifierParser()
        {
            var expressionParser = FakeParser.ForExpressions();
            return new ParameterParser(expressionParser);
        }
    }
}
