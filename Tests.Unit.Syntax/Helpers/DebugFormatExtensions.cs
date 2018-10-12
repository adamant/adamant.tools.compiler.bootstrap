using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Helpers
{
    public static class DebugFormatExtensions
    {
        [NotNull]
        public static string DebugFormat([NotNull][ItemNotNull] this IEnumerable<Diagnostic> diagnostics)
        {
            return string.Join(", ",
                diagnostics.Select(d =>
                    $"{d.ErrorCode}@{d.StartPosition.Line}:{d.StartPosition.Column}"))
                .AssertNotNull();
        }

        [NotNull]
        public static string DebugFormat([NotNull][ItemCanBeNull] this IEnumerable<PsuedoToken> tokens)
        {
            return string.Join(", ", tokens).AssertNotNull();
        }
    }
}
