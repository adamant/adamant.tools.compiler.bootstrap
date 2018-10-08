using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Tests;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes;
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
            var tokens = FakeTokenStream($"");

            var cu = Parse(tokens);

            Assert.Null(cu.Namespace);
            Assert.Empty(cu.UsingDirectives);
            Assert.Empty(cu.Declarations);
        }

        [Fact]
        public void Namespace_only()
        {
            var fakeNameSyntax = new FakeNameSyntax();
            var tokens = FakeTokenStream($"namespace {fakeNameSyntax};");

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
            var tokens = FakeTokenStream($"using {fakeUsingSyntax}");

            var cu = Parse(tokens);

            Assert.Null(cu.Namespace);
            Assert.Equal(fakeUsingSyntax, cu.UsingDirectives.Single());
            Assert.Empty(cu.Declarations);
        }

        private static FakeTokenStream FakeTokenStream(FormattableString tokenDescription)
        {
            var file = tokenDescription.Format.ToFakeCodeFile();
            var tokens = CreateFakeTokens(new Lexer().Lex(file), tokenDescription.GetArguments());
            return new FakeTokenStream(file, tokens);
        }

        private static IEnumerable<Token> CreateFakeTokens(
            IEnumerable<Token> tokens,
            object[] fakeTokenValues)
        {
            using (var enumerator = tokens.GetEnumerator())
                while (enumerator.MoveNext())
                {
                    switch (enumerator.Current.Kind)
                    {
                        case TokenKind.OpenBrace:
                            var startSpan = enumerator.Current.Span;
                            Assert.True(enumerator.MoveNext());
                            if (enumerator.Current.Kind == TokenKind.OpenBrace)
                            {
                                // Escaped open brace
                                yield return enumerator.Current;
                            }
                            else
                            {
                                Assert.Equal(TokenKind.IntegerLiteral, enumerator.Current.Kind);
                                var placeholder = (IntegerLiteralToken)enumerator.Current;
                                Assert.True(enumerator.MoveNext());
                                Assert.Equal(TokenKind.CloseBrace, enumerator.Current.Kind);
                                yield return new Token(FakeToken.Kind,
                                    TextSpan.Covering(startSpan, enumerator.Current.Span),
                                    fakeTokenValues[(int)placeholder.Value]);
                            }
                            break;
                        case TokenKind.CloseBrace:
                            // Escaped close brace
                            Assert.True(enumerator.MoveNext());
                            Assert.Equal(TokenKind.CloseBrace, enumerator.Current.Kind);
                            yield return enumerator.Current;
                            break;
                        case TokenKind.Whitespace:
                        case TokenKind.Comment:
                            // Skip
                            break;
                        default:
                            yield return enumerator.Current;
                            break;
                    }
                }
        }

        private static CompilationUnitSyntax Parse(ITokenStream tokenStream)
        {
            var parser = NewCompilationUnitParser();
            return parser.Parse(tokenStream);
        }

        private static CompilationUnitParser NewCompilationUnitParser()
        {
            var usingDirectiveParser = new FakeUsingDirectiveParser();
            var declarationParser = new FakeDeclarationParser();
            var qualifiedNameParser = new FakeQualifiedNameParser();
            return new CompilationUnitParser(usingDirectiveParser, declarationParser,
                qualifiedNameParser);
        }

        private static Token Token(TokenKind kind, int start, int length, object value = null)
        {
            return new Token(kind, new TextSpan(start, length), value);
        }
    }
}
