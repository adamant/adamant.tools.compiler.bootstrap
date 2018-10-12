using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
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

            Assert.Null(p.VarKeyword);
            Assert.Equal(tokens[0], p.Name);
            Assert.Equal(tokens[1], p.Colon);
            Assert.Equal(type, p.TypeExpression);
        }

        [NotNull]
        private static ParameterSyntax ParseWithoutError([NotNull] ITokenStream tokens)
        {
            var parser = NewAccessModifierParser();
            var diagnostics = new DiagnosticsBuilder();
            var parameterSyntax = parser.Parse(tokens, diagnostics);
            Assert.Empty(diagnostics.Build());
            return parameterSyntax;
        }

        [NotNull]
        private static ParameterParser NewAccessModifierParser()
        {
            var expressionParser = FakeParser.For<ExpressionSyntax>();
            return new ParameterParser(expressionParser);
        }
    }
}
