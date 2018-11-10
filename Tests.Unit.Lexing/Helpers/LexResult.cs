using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Helpers
{
    public class LexResult
    {
        [NotNull] public readonly CodeFile File;
        [NotNull] public readonly FixedList<IToken> Tokens;
        [NotNull] public readonly FixedList<Diagnostic> Diagnostics;

        public LexResult([NotNull] ITokenIterator iterator)
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

        [NotNull]
        public IToken AssertSingleToken()
        {
            Assert.True(2 == Tokens.Count, $"Expected token count {1}, was {Tokens.Count - 1} (excluding EOF)");
            var eof = Tokens.Last().AssertOfType<IEndOfFileToken>();
            Assert.Equal(new TextSpan(Tokens[Tokens.Count - 2].NotNull().Span.End, 0), eof.Span);
            return Tokens[0].NotNull();
        }

        public void AssertTokens(int expectedCount)
        {
            Assert.Equal(expectedCount + 1, Tokens.Count);
        }

        public void AssertNoDiagnostics()
        {
            Assert.Empty(Diagnostics);
        }

        [NotNull]
        public Diagnostic AssertSingleDiagnostic()
        {
            return Assert.Single(Diagnostics).NotNull();
        }

        public void AssertDiagnostics(int expectedCount)
        {
            Assert.Equal(expectedCount, Diagnostics.Count);
        }

        [NotNull]
        public string TokensToString()
        {
            return string.Concat(Tokens.Select(t => t.Text(File.Code))).NotNull();
        }

        [NotNull]
        public FixedList<PsuedoToken> ToPsuedoTokens()
        {
            return Tokens.Select(t => PsuedoToken.For(t.NotNull(), File.Code)).ToFixedList();
        }
    }
}