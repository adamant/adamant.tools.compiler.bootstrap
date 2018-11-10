using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class FunctionBodyParser
    {
        [MustUseReturnValue]
        [NotNull]
        private static PatternSyntax ParsePattern(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var pattern = ParsePatternAtom(tokens, diagnostics);
            for (; ; )
            {
                switch (tokens.Current)
                {
                    case IPipeToken pipe:
                        tokens.Next();
                        var rightOperand = ParsePatternAtom(tokens, diagnostics);
                        pattern = new OrPatternSyntax(pattern, pipe, rightOperand);
                        break;
                    default:
                        return pattern;
                }
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private static PatternSyntax ParsePatternAtom(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            switch (tokens.Current)
            {
                case IIdentifierToken identifier:
                {
                    tokens.Next();
                    return new AnyPatternSyntax(identifier);
                }
                case IDotToken dotToken:
                {
                    tokens.Next();
                    var identifier = tokens.ExpectIdentifier();
                    return new EnumValuePatternSyntax(dotToken, identifier);
                }
                default:
                    throw NonExhaustiveMatchException.For(tokens.Current);
            }
        }
    }
}
