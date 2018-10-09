using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public class FakeListParser : IListParser
    {
        public SyntaxList<T> ParseList<T, TTerminator>(
            ITokenStream tokens,
            ParseFunction<T> parseItem,
            TypeOf<TTerminator> terminatorType) where T : SyntaxNode where TTerminator : Token
        {
            throw new System.NotImplementedException();
        }

        public SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator, TTerminator>(
            ITokenStream tokens,
            ParseFunction<T> parseItem,
            TypeOf<TSeparator> separatorType,
            TypeOf<TTerminator> terminatorType) where T : SyntaxNode where TSeparator : Token where TTerminator : Token
        {
            throw new System.NotImplementedException();
        }
    }
}
