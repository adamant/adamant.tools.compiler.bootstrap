using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests
{
    public static class AssertExtensions
    {
        public static void AssertIs(
            this Token token,
            TokenKind expectedKind,
            int expectedStart,
            int expectedLength,
            object expectedValue = null)
        {
            Assert.Equal(expectedKind, token.Kind);
            Assert.True(expectedStart == token.Span.Start, $"Expected token start {expectedStart}, was {token.Span.Start}");
            Assert.True(expectedLength == token.Span.Length, $"Expected token length {expectedLength}, was {token.Span.Length}");
            Assert.Equal(expectedValue, token.Value);
        }

        public static (List<Token>, IReadOnlyList<Diagnostic>) AssertCount(
            this IEnumerable<Token> tokens,
            int count)
        {
            var list = tokens.ToList();
            Assert.True(count + 1 == list.Count, $"Expected token count {count}, was {list.Count - 1} (excluding EOF)");
            var eof = list.Last();
            Assert.Equal(TokenKind.EndOfFile, eof.Kind);
            Assert.Equal(new TextSpan(list[list.Count - 2].Span.End, 0), eof.Span);
            var diagnostics = (IReadOnlyList<Diagnostic>)eof.Value;
            return (list, diagnostics);
        }

        public static Token AssertSingleNoErrors(this IEnumerable<Token> tokens)
        {
            var (tokensList, diagnostics) = tokens.AssertCount(1);
            Assert.Empty(diagnostics);
            return tokensList[0];
        }

        public static (Token, IReadOnlyList<Diagnostic>) AssertSingleWithErrors(this IEnumerable<Token> tokens)
        {
            var (tokensList, diagnostics) = tokens.AssertCount(1);
            Assert.NotEmpty(diagnostics);
            return (tokensList[0], diagnostics);
        }

        public static List<Diagnostic> AssertCount(
            this IEnumerable<Diagnostic> diagnostics,
            int count)
        {
            var list = diagnostics.OrderBy(d => d.Span).ToList();
            Assert.True(count == list.Count, $"Expected diagnostics count {count}, was {list.Count}: {list.DebugFormat()}");
            return list;
        }

        public static Diagnostic AssertSingle(this IReadOnlyList<Diagnostic> diagnostics)
        {
            Assert.True(diagnostics.Count == 1, $"Expected single diagnostic, were {diagnostics.Count}");
            return diagnostics[0];
        }

        public static void AssertError(this Diagnostic diagnostic, int errorCode, int start, int length)
        {
            Assert.Equal(DiagnosticLevel.CompilationError, diagnostic.Level);
            AssertDiagnostic(diagnostic, errorCode, start, length);
        }

        public static void AssertDiagnostic(this Diagnostic diagnostic, int errorCode, int start, int length)
        {
            Assert.Equal(DiagnosticPhase.Lexing, diagnostic.Phase);
            Assert.Equal(errorCode, diagnostic.ErrorCode);
            Assert.True(start == diagnostic.Span.Start, $"Expected diagnostic start {start}, was {diagnostic.Span.Start}");
            Assert.True(length == diagnostic.Span.Length, $"Expected diagnostic length {length}, was {diagnostic.Span.Length}");
        }
    }
}
