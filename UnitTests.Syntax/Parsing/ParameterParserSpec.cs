using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
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
    public class ParameterParserSpec
    {
        [Fact]
        public void Unused_parameter()
        {
            var type = Fake.Name();
            var tokens = FakeTokenStream.From($"_: {type};");

            var p = Parse(tokens);

            Assert.Null(p.VarKeyword);
            Assert.Equal(tokens[0], p.Name);
            Assert.Equal(tokens[1], p.Colon);
            Assert.Equal(type, p.TypeExpression);
        }

        [NotNull]
        private static ParameterSyntax Parse([NotNull] ITokenStream tokenStream)
        {
            var parser = NewAccessModifierParser();
            return parser.Parse(tokenStream);
        }

        [NotNull]
        private static ParameterParser NewAccessModifierParser()
        {
            var expressionParser = Fake.Parser<ExpressionSyntax>();
            return new ParameterParser(expressionParser);
        }
    }
}
