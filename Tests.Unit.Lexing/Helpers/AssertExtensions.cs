using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Helpers
{
    // TODO use xUnit assert extensions
    public static class AssertExtensions
    {
        public static void AssertIdentifier(
            this IToken token,
            int expectedStart,
            int expectedLength,
            string expectedValue)
        {
            Assert.NotNull(token);
            var identifier = Assert.OfType<IIdentifierToken>(token);
            Assert.True(expectedStart == identifier.Span.Start, $"Expected token start {expectedStart}, was {identifier.Span.Start}");
            Assert.True(expectedLength == identifier.Span.Length, $"Expected token length {expectedLength}, was {identifier.Span.Length}");
            Assert.Equal(expectedValue, identifier.Value);
        }

        public static void AssertStringLiteral(
            this IToken token,
            int expectedStart,
            int expectedLength,
            string expectedValue)
        {
            Assert.NotNull(token);
            var identifier = Assert.OfType<IStringLiteralToken>(token);
            Assert.True(expectedStart == identifier.Span.Start, $"Expected token start {expectedStart}, was {identifier.Span.Start}");
            Assert.True(expectedLength == identifier.Span.Length, $"Expected token length {expectedLength}, was {identifier.Span.Length}");
            Assert.Equal(expectedValue, identifier.Value);
        }

        public static void AssertIs<T>(
            this IToken token,
            int expectedStart,
            int expectedLength)
            where T : IToken
        {
            Assert.NotNull(token);
            Assert.OfType<T>(token);
            Assert.True(expectedStart == token.Span.Start, $"Expected token start {expectedStart}, was {token.Span.Start}");
            Assert.True(expectedLength == token.Span.Length, $"Expected token length {expectedLength}, was {token.Span.Length}");
        }

        public static void AssertError(this Diagnostic diagnostic, int errorCode, int start, int length)
        {
            Assert.NotNull(diagnostic);
            Assert.Equal(DiagnosticLevel.CompilationError, diagnostic.Level);
            AssertLexingDiagnostic(diagnostic, errorCode, start, length);
        }

        public static void AssertLexingDiagnostic(this Diagnostic diagnostic, int errorCode, int start, int length)
        {
            Assert.Diagnostic(diagnostic, DiagnosticPhase.Lexing, errorCode, start, length);
        }
    }
}
