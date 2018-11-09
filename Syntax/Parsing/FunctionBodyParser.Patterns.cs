using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public partial class FunctionBodyParser
    {
        [MustUseReturnValue]
        [NotNull]
        private static PatternSyntax ParsePattern(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var pattern = ParsePatternAtom(tokens, diagnostics);
            for (; ; )
            {
                switch (tokens.Current)
                {
                    case PipeToken pipe:
                        tokens.MoveNext();
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
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (tokens.Current)
            {
                case IdentifierToken identifier:
                    {
                        tokens.MoveNext();
                        return new AnyPatternSyntax(identifier);
                    }
                case DotToken dotToken:
                    {
                        tokens.MoveNext();
                        var identifier = tokens.ExpectIdentifier();
                        return new EnumValuePatternSyntax(dotToken, identifier);
                    }
                default:
                    throw NonExhaustiveMatchException.For(tokens.Current);
            }
        }
    }
}
