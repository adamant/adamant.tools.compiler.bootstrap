using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Helpers
{
    public class LexResult
    {
        public CodeFile File { get; }
        public FixedList<IToken> Tokens { get; }
        public FixedList<Diagnostic> Diagnostics { get; }

        public LexResult(ITokenIterator<IToken> iterator)
        {
            var tokens = new List<IToken>();
            do
            {
                tokens.Add(iterator.Current);
            } while (iterator.Next());

            File = iterator.Context.File;
            Tokens = tokens.ToFixedList();
            Diagnostics = iterator.Context.Diagnostics.Build();
        }

        public IToken AssertSingleToken()
        {
            Assert.True(2 == Tokens.Count, $"Expected token count {1}, was {Tokens.Count - 1} (excluding EOF)");
            var eof = Tokens[^1].AssertOfType<IEndOfFileToken>();
            Assert.Equal(new TextSpan(Tokens[^2].Span.End, 0), eof.Span);
            return Tokens[0];
        }

        public void AssertTokens(int expectedCount)
        {
            Assert.Equal(expectedCount + 1, Tokens.Count);
        }

        public void AssertNoDiagnostics()
        {
            Assert.Empty(Diagnostics);
        }

        public Diagnostic AssertSingleDiagnostic()
        {
            return Assert.Single(Diagnostics);
        }

        public void AssertDiagnostics(int expectedCount)
        {
            Assert.Equal(expectedCount, Diagnostics.Count);
        }

        public string TokensToString()
        {
            return string.Concat(Tokens.Select(t => t.Text(File.Code)));
        }

        public FixedList<PsuedoToken> ToPsuedoTokens()
        {
            return Tokens.Select(t => PsuedoToken.For(t, File.Code)).ToFixedList();
        }
    }
}
