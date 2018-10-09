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

        [NotNull]
        private static FakeTokenStream FakeTokenStream([NotNull] FormattableString tokenDescription)
        {
            var file = tokenDescription.Format.ToFakeCodeFile();
            var tokens = CreateFakeTokens(new Lexer().Lex(file), tokenDescription.GetArguments());
            return new FakeTokenStream(file, tokens);
        }

        [NotNull]
        private static IEnumerable<Token> CreateFakeTokens(
            [NotNull][ItemNotNull] IEnumerable<Token> tokens,
            [NotNull] IReadOnlyList<object> fakeTokenValues)
        {
            using (var enumerator = tokens.GetEnumerator())
                while (enumerator.MoveNext())
                {
                    switch (enumerator.Current)
                    {
                        case OpenBraceToken _:
                            var startSpan = enumerator.Current.Span;
                            Assert.True(enumerator.MoveNext());
                            if (enumerator.Current is OpenBraceToken)
                            {
                                // Escaped open brace
                                yield return enumerator.Current;
                            }
                            else
                            {
                                var placeholder = (int)Assert.IsType<IntegerLiteralToken>(enumerator.Current).Value;
                                Assert.True(enumerator.MoveNext());
                                Assert.IsType<CloseBraceToken>(enumerator.Current);
                                var value = (SyntaxNode)fakeTokenValues[placeholder];
                                yield return new FakeToken(TextSpan.Covering(startSpan, enumerator.Current.Span), value);
                            }
                            break;
                        case CloseBraceToken _:
                            // Escaped close brace
                            Assert.True(enumerator.MoveNext());
                            Assert.IsType<CloseBraceToken>(enumerator.Current);
                            yield return enumerator.Current;
                            break;
                        case WhitespaceToken _:
                        case CommentToken _:
                            // Skip
                            break;
                        default:
                            yield return enumerator.Current;
                            break;
                    }
                }
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
