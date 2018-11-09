using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface IEndOfFileToken : IToken
    {
        [NotNull]
        Diagnostics Diagnostics { get; }
    }
}
