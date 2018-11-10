using System;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class ParseFailedException : Exception
    {
        public ParseFailedException(string message)
            : base(message)
        {
        }
    }
}
