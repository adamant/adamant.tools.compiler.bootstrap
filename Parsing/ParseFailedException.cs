using System;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    /// <summary>
    /// Used as control flow within the parser. When a parse error requiring us
    /// to jump out to a higher syntax node occurs, this is the exception that
    /// is thrown.
    /// </summary>
    public class ParseFailedException : Exception
    {
        public ParseFailedException()
        {
        }

        public ParseFailedException(string message)
            : base(message)
        {
        }
    }
}
