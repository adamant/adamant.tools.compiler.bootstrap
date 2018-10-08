using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing
{
    public interface ITokenStream
    {
        CodeFile File { get; }
        Token? Current { get; }
        bool Next();
    }
}
