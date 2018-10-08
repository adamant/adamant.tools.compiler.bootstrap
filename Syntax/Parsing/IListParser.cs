using System;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public interface IListParser
    {
        [MustUseReturnValue]
        SyntaxList<T> ParseList<T>(
            ITokenStream tokens,
            Func<ITokenStream, T> parseItem,
            TokenKind terminator)
            where T : SyntaxNode;

        [MustUseReturnValue]
        SeparatedListSyntax<T> ParseSeparatedList<T>(
            ITokenStream tokens,
            Func<ITokenStream, T> parseItem,
            TokenKind separator,
            TokenKind terminator)
            where T : SyntaxNode;
    }
}
