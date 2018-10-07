using Adamant.Tools.Compiler.Bootstrap.Core.Tests;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Framework;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests
{
    [Category("Parse")]
    public class ParserSpec : SyntaxFactory
    {
        [Fact]
        public void Round_trip_bool_parameter()
        {
            const string code = @"public fn func(_: bool) -> void
                                {
                                }
                                ";
            var file = code.ToFakeCodeFile();

            var tokens = new Lexer().Lex(file);
            var parser = new Parser();
            var actual = parser.Parse(file, tokens);

            var function = Function(Public(), "func", Parameters(), Void());
            var expected = CompilationUnit(function);
            Assert.Equal(expected, actual);
        }
    }
}
