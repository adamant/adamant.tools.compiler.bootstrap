using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    // Delegate needed so we can declare the arg as not null
    [NotNull]
    public delegate T ParseFunction<out T>([NotNull] ITokenStream stream)
        where T : SyntaxNode;

    public interface IListParser
    {
        [MustUseReturnValue]
        [NotNull]
        SyntaxList<T> ParseList<T, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            TypeOf<TTerminator> terminatorType)
            where T : SyntaxNode
            where TTerminator : Token;

        [MustUseReturnValue]
        [NotNull]
        SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            TypeOf<TSeparator> separatorType,
            TypeOf<TTerminator> terminatorType)
            where T : SyntaxNode
            where TSeparator : Token
            where TTerminator : Token;
    }
}
