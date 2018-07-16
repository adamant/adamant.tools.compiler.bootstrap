using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class StringToken : Token
    {
        public readonly string Value;

        public StringToken(CodeText code, TextSpan span, string value, IEnumerable<DiagnosticInfo> diagnosticInfos)
            : base(code, span, TokenKind.StringLiteral, false, diagnosticInfos)
        {
            Value = value;
        }
    }
}
