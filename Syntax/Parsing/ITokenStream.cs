using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public interface ITokenStream
    {
        CodeReference CodeReference { get; }
        CodeText Code { get; }
        bool Finished { get; }
        Token Current { get; }
        Token Consume();
    }
}
