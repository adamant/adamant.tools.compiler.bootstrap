using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public interface ITokenIterator
    {
        [NotNull] ParseContext Context { get; }
        bool Next();
        [CanBeNull] IToken Current { get; }
    }
}
