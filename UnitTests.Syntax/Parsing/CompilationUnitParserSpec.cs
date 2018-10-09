using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Parsing
{
    [UnitTest]
    [Category("Parse")]
    public class CompilationUnitParserSpec
    {
        [Fact]
        public void Empty_file()
        {
            var tokens = FakeTokenStream.From($"");

            var cu = Parse(tokens);

            Assert.Null(cu.Namespace);
            Assert.Empty(cu.UsingDirectives);
            Assert.Empty(cu.Declarations);
        }

        [Fact]
        public void Namespace_only()
        {
            var fakeNameSyntax = new FakeNameSyntax();
            var tokens = FakeTokenStream.From($"namespace {fakeNameSyntax};");

            var cu = Parse(tokens);

            var ns = cu.Namespace;
            Assert.NotNull(ns);
            Assert.Equal(tokens[0], ns.NamespaceKeyword);
            Assert.Equal(fakeNameSyntax, ns.Name);
            Assert.Equal(tokens[2], ns.Semicolon);

            Assert.Empty(cu.UsingDirectives);
            Assert.Empty(cu.Declarations);
        }

        [Fact]
        public void Using_only()
        {
            var fakeUsingSyntax = new FakeUsingDirectiveSyntax();
            var tokens = FakeTokenStream.From($"using {fakeUsingSyntax}");

            var cu = Parse(tokens);

            Assert.Null(cu.Namespace);
            Assert.Equal(fakeUsingSyntax, cu.UsingDirectives.Single());
            Assert.Empty(cu.Declarations);
        }

        [NotNull]
        private static CompilationUnitSyntax Parse([NotNull] ITokenStream tokenStream)
        {
            var parser = NewCompilationUnitParser();
            return parser.Parse(tokenStream);
        }

        [NotNull]
        private static CompilationUnitParser NewCompilationUnitParser()
        {
            var usingDirectiveParser = new FakeUsingDirectiveParser();
            var declarationParser = new FakeDeclarationParser();
            var qualifiedNameParser = new FakeQualifiedNameParser();
            return new CompilationUnitParser(usingDirectiveParser, declarationParser,
                qualifiedNameParser);
        }
    }
}
