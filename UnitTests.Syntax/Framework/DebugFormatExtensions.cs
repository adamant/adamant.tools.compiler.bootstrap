using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Framework
{
    public static class DebugFormatExtensions
    {
        public static string DebugFormat([NotNull] this IEnumerable<Diagnostic> diagnostics)
        {
            return string.Join(", ",
                diagnostics.Select(d =>
                    $"{d.ErrorCode}@{d.StartPosition.Line}:{d.StartPosition.Column}"));
        }

        public static string DebugFormat([NotNull] this IEnumerable<PsuedoToken> tokens)
        {
            return string.Join(", ", tokens);
        }
    }
}
