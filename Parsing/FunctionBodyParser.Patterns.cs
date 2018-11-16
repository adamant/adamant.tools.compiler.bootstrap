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
                    case IPipeToken _:
                        Tokens.Expect<IPipeToken>();
                        var rightOperand = ParsePatternAtom();
                        pattern = new OrPatternSyntax(pattern, rightOperand);
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
                case IIdentifierToken _:
                    return new AnyPatternSyntax(Tokens.RequiredToken<IIdentifierToken>());
                case IDotToken _:
                {
                    Tokens.Expect<IDotToken>();
                    var identifier = Tokens.RequiredToken<IIdentifierToken>();
                    return new EnumValuePatternSyntax(identifier);
                }
                default:
                    throw NonExhaustiveMatchException.For(Tokens.Current);
            }
        }
    }
}
