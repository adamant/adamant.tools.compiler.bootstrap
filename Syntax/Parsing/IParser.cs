using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public interface IParser<out T>
        where T : SyntaxNode
    {
        /// <summary>
        /// The parse method must be called when an appropriate nonterminal is
        /// expected. It must always return an instance of <see cref="T"/>. That
        /// instance may represent an invalid value. It must always advance the
        /// token stream.
        /// </summary>
        /// <param name="tokens">A token stream without whitespace or comments</param>
        [MustUseReturnValue]
        T Parse(ITokenStream tokens);
    }
}
