using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface IEndOfFileToken : IToken
    {
        [NotNull]
        FixedList<Diagnostic> Diagnostics { get; }
    }
}
