using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Parsing
{
    [UnitTest]
    [Category("Parse")]
    public class CompilationUnitParserSpec
    {
        [Fact]
        public void With_namespace()
        {
            var name = FakeSyntax.Name();
            var usings = FakeSyntax.List<UsingDirectiveSyntax>();
            var declarations = FakeSyntax.List<DeclarationSyntax>();
            var tokens = FakeTokenStream.From($"namespace {name};{usings}{declarations}");

            var cu = ParseWithoutError(tokens);

            var ns = cu.Namespace;
            Assert.NotNull(ns);
            Assert.Equal(tokens[0], ns.NamespaceKeyword);
            Assert.Equal(name, ns.Name);
            Assert.Equal(tokens[2], ns.Semicolon);

            Assert.Empty(cu.Namespace.UsingDirectives);
            Assert.Empty(cu.Namespace.Declarations);
            Assert.Empty(cu.Diagnostics);
        }

        [Fact]
        public void No_namespace()
        {
            var usings = FakeSyntax.List<UsingDirectiveSyntax>();
            var declarations = FakeSyntax.List<DeclarationSyntax>();
            var tokens = FakeTokenStream.From($"{usings}{declarations}");

            var cu = ParseWithoutError(tokens);

            Assert.Null(cu.Namespace.Name);
            Assert.Equal(usings, cu.Namespace.UsingDirectives);
            Assert.Equal(declarations, cu.Namespace.Declarations);
            Assert.Empty(cu.Diagnostics);
        }

        [NotNull]
        private static CompilationUnitSyntax ParseWithoutError([NotNull] ITokenStream tokenStream)
        {
            var parser = NewCompilationUnitParser();
            return parser.ParseCompilationUnit(tokenStream);
        }

        [NotNull]
        private static CompilationUnitParser NewCompilationUnitParser()
        {
            var usingDirectiveParser = FakeParser.ForUsingDirectives();
            var declarationParser = FakeParser.ForDeclarations();
            var nameParser = FakeParser.ForNames();
            return new CompilationUnitParser(
                usingDirectiveParser,
                declarationParser,
                nameParser);
        }
    }
}
