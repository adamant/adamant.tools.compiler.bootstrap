using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class FunctionBodyParser
    {
        [MustUseReturnValue]
        [NotNull]
        private PatternSyntax ParsePattern()
        {
            var pattern = ParsePatternAtom();
            for (; ; )
            {
                switch (Tokens.Current)
                {
                    case IPipeToken pipe:
                        Tokens.Next();
                        var rightOperand = ParsePatternAtom();
                        pattern = new OrPatternSyntax(pattern, pipe, rightOperand);
                        break;
                    default:
                        return pattern;
                }
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private PatternSyntax ParsePatternAtom()
        {
            switch (Tokens.Current)
            {
                case IIdentifierToken identifier:
                {
                    Tokens.Next();
                    return new AnyPatternSyntax(identifier);
                }
                case IDotToken dotToken:
                {
                    Tokens.Next();
                    var identifier = Tokens.ExpectIdentifier();
                    return new EnumValuePatternSyntax(dotToken, identifier);
                }
                default:
                    throw NonExhaustiveMatchException.For(Tokens.Current);
            }
        }
    }
}
