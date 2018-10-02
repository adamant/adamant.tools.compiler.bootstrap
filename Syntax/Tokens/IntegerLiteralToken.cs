using System.Collections.Generic;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Core.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class IntegerLiteralToken : Token
    {
        public readonly BigInteger Value;

        public IntegerLiteralToken(CodeFile file, TextSpan span, bool isMissing, BigInteger value, IEnumerable<DiagnosticInfo> diagnosticInfos)
            : base(file, span, TokenKind.IntegerLiteral, isMissing, diagnosticInfos)
        {
            Value = value;
        }
    }
}
