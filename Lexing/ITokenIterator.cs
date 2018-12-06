using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public interface ITokenIterator
    {
        ParseContext Context { get; }

        bool Next();

        /// <exception cref="InvalidOperationException">If current is accessed after Next() has returned false</exception>
        IToken Current { get; }
    }
}
