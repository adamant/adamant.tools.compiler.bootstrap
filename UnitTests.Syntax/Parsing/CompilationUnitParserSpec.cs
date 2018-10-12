using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
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

            var cu = ParseWithoutError(tokens);

            Assert.Null(cu.Namespace);
            Assert.Empty(cu.UsingDirectives);
            Assert.Empty(cu.Declarations);
            Assert.Empty(cu.Diagnostics);
        }

        [Fact]
        public void Namespace_only()
        {
            var name = FakeSyntax.Name();
            var tokens = FakeTokenStream.From($"namespace {name};");

            var cu = ParseWithoutError(tokens);

            var ns = cu.Namespace;
            Assert.NotNull(ns);
            Assert.Equal(tokens[0], ns.NamespaceKeyword);
            Assert.Equal(name, ns.Name);
            Assert.Equal(tokens[2], ns.Semicolon);

            Assert.Empty(cu.UsingDirectives);
            Assert.Empty(cu.Declarations);
            Assert.Empty(cu.Diagnostics);
        }

        [Fact]
        public void Using_only()
        {
            var @using = FakeSyntax.UsingDirective();
            var tokens = FakeTokenStream.From($"using {@using}");

            var cu = ParseWithoutError(tokens);

            Assert.Null(cu.Namespace);
            Assert.Equal(@using, cu.UsingDirectives.Single());
            Assert.Empty(cu.Declarations);
            Assert.Empty(cu.Diagnostics);
        }

        [NotNull]
        private static CompilationUnitSyntax ParseWithoutError([NotNull] ITokenStream tokenStream)
        {
            var parser = NewCompilationUnitParser();
            return parser.Parse(tokenStream);
        }

        [NotNull]
        private static CompilationUnitParser NewCompilationUnitParser()
        {
            var usingDirectiveParser = FakeParser.For<UsingDirectiveSyntax>();
            var declarationParser = FakeParser.For<DeclarationSyntax>();
            var qualifiedNameParser = FakeParser.For<NameSyntax>();
            return new CompilationUnitParser(
                usingDirectiveParser,
                declarationParser,
                qualifiedNameParser);
        }
    }
}
