using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class StringToken : Token
    {
        public readonly string Value;

        public StringToken(SourceText source, TextSpan span, string value, IEnumerable<DiagnosticInfo> diagnosticInfos)
            : base(source, span, TokenKind.StringLiteral, false, diagnosticInfos)
        {
            Value = value;
        }
    }
}
