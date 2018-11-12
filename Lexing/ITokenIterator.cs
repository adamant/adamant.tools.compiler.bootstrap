using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public interface ITokenIterator
    {
        [NotNull] ParseContext Context { get; }

        bool Next();

        /// <exception cref="InvalidOperationException">If current is accessed after Next() has returned false</exception>
        [NotNull] IToken Current { get; }
    }
}
