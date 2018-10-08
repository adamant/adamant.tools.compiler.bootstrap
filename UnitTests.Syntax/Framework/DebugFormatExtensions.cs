using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Framework
{
    public static class DebugFormatExtensions
    {
        public static string DebugFormat(this IEnumerable<Diagnostic> diagnostics)
        {
            return string.Join(", ",
                diagnostics.Select(d =>
                    $"{d.ErrorCode}@{d.StartPosition.Line}:{d.StartPosition.Column}"));
        }

        public static string DebugFormat(this IEnumerable<PsuedoToken> tokens)
        {
            return string.Join(", ", tokens);
        }
    }
}
