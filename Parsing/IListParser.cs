using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    // Delegate needed so we can declare the arg as not null
    [NotNull]
    public delegate T ParseFunction<out T>([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        where T : SyntaxNode;

    [CanBeNull]
    public delegate T AcceptFunction<out T>([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        where T : SyntaxNode;

    // TODO list parsing based on a terminator is problematic, it would be better to have the parse function decide if the next token was a start
    public interface IListParser
    {
        [MustUseReturnValue]
        [NotNull]
        SyntaxList<T> ParseList<T>(
            [NotNull] ITokenStream tokens,
            [NotNull] AcceptFunction<T> acceptItem,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : SyntaxNode;

        [MustUseReturnValue]
        [NotNull]
        SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator>(
            [NotNull] ITokenStream tokens,
            [NotNull] AcceptFunction<T> acceptItem,
            Type<TSeparator> separatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : SyntaxNode
            where TSeparator : Token;

        [Obsolete("Use ParseList() taking an AcceptFunction instead")]
        [MustUseReturnValue]
        [NotNull]
        SyntaxList<T> ParseList<T, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TTerminator> terminatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : SyntaxNode
            where TTerminator : Token;

        [Obsolete("Use ParseSeparatedList() taking an AcceptFunction instead")]
        [MustUseReturnValue]
        [NotNull]
        SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TSeparator> separatorType,
            Type<TTerminator> terminatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : SyntaxNode
            where TSeparator : Token
            where TTerminator : Token;
    }
}
