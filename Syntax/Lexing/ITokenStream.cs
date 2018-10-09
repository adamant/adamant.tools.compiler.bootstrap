using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing
{
    /// <summary>
    /// A stream of tokens form a <see cref="CodeFile"/>. Never includes
    /// <see cref="TriviaToken"/>s.
    /// </summary>
    public interface ITokenStream : IEnumerator<Token>
    {
        [NotNull]
        CodeFile File { get; }
    }
}
