using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public interface ITokenIterator<out TToken>
        where TToken : class, IToken
    {
        ParseContext Context { get; }

        bool Next();

        /// <exception cref="InvalidOperationException">If current is accessed after Next() has returned false</exception>
        TToken Current { get; }
    }
}
