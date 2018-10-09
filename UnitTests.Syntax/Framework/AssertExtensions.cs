using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Framework
{
    public static class AssertExtensions
    {
        public static void AssertIdentifier(
            [CanBeNull] this Token token,
            int expectedStart,
            int expectedLength,
            string expectedValue)
        {
            Assert.NotNull(token);
            var identifier = Assert.IsAssignableFrom<IdentifierToken>(token);
            Assert.True(expectedStart == identifier.Span.Start, $"Expected token start {expectedStart}, was {identifier.Span.Start}");
            Assert.True(expectedLength == identifier.Span.Length, $"Expected token length {expectedLength}, was {identifier.Span.Length}");
            Assert.Equal(expectedValue, identifier.Value);
        }

        public static void AssertStringLiteral(
            [CanBeNull] this Token token,
            int expectedStart,
            int expectedLength,
            string expectedValue)
        {
            Assert.NotNull(token);
            var identifier = Assert.IsType<StringLiteralToken>(token);
            Assert.True(expectedStart == identifier.Span.Start, $"Expected token start {expectedStart}, was {identifier.Span.Start}");
            Assert.True(expectedLength == identifier.Span.Length, $"Expected token length {expectedLength}, was {identifier.Span.Length}");
            Assert.Equal(expectedValue, identifier.Value);
        }

        public static void AssertIs<T>(
            [CanBeNull] this Token token,
            int expectedStart,
            int expectedLength)
            where T : Token
        {
            Assert.NotNull(token);
            Assert.IsType<T>(token);
            Assert.True(expectedStart == token.Span.Start, $"Expected token start {expectedStart}, was {token.Span.Start}");
            Assert.True(expectedLength == token.Span.Length, $"Expected token length {expectedLength}, was {token.Span.Length}");
        }

        public static (List<Token>, IReadOnlyList<Diagnostic>) AssertCount(
            [NotNull][ItemCanBeNull] this IEnumerable<Token> tokens,
            int count)
        {
            var list = tokens.ToList();
            Assert.True(count + 1 == list.Count, $"Expected token count {count}, was {list.Count - 1} (excluding EOF)");
            var eof = Assert.IsType<EndOfFileToken>(list.Last());
            Assert.Equal(new TextSpan(list[list.Count - 2].Span.End, 0), eof.Span);
            return (list, eof.Diagnostics);
        }

        [NotNull]
        public static Token AssertSingleNoErrors([NotNull][ItemNotNull] this IEnumerable<Token> tokens)
        {
            var (tokensList, diagnostics) = tokens.AssertCount(1);
            Assert.Empty(diagnostics);
            return tokensList[0];
        }

        public static (Token, IReadOnlyList<Diagnostic>) AssertSingleWithErrors([NotNull][ItemCanBeNull] this IEnumerable<Token> tokens)
        {
            var (tokensList, diagnostics) = tokens.AssertCount(1);
            Assert.NotEmpty(diagnostics);
            return (tokensList[0], diagnostics);
        }

        [NotNull]
        [ItemNotNull]
        public static List<Diagnostic> AssertCount(
            [NotNull][ItemNotNull] this IEnumerable<Diagnostic> diagnostics,
            int count)
        {
            var list = diagnostics.OrderBy(d => d.Span).ToList();
            Assert.True(count == list.Count, $"Expected diagnostics count {count}, was {list.Count}: {list.DebugFormat()}");
            return list;
        }

        [NotNull]
        public static Diagnostic AssertSingle([NotNull][ItemNotNull] this IReadOnlyList<Diagnostic> diagnostics)
        {
            Assert.True(diagnostics.Count == 1, $"Expected single diagnostic, were {diagnostics.Count}");
            return diagnostics[0];
        }

        public static void AssertError([CanBeNull] this Diagnostic diagnostic, int errorCode, int start, int length)
        {
            Assert.NotNull(diagnostic);
            Assert.Equal(DiagnosticLevel.CompilationError, diagnostic.Level);
            AssertDiagnostic(diagnostic, errorCode, start, length);
        }

        public static void AssertDiagnostic([CanBeNull] this Diagnostic diagnostic, int errorCode, int start, int length)
        {
            Assert.NotNull(diagnostic);
            Assert.Equal(DiagnosticPhase.Lexing, diagnostic.Phase);
            Assert.Equal(errorCode, diagnostic.ErrorCode);
            Assert.True(start == diagnostic.Span.Start, $"Expected diagnostic start {start}, was {diagnostic.Span.Start}");
            Assert.True(length == diagnostic.Span.Length, $"Expected diagnostic length {length}, was {diagnostic.Span.Length}");
        }
    }
}
