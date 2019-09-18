namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        //private PatternSyntax ParsePattern()
        //{
        //    var pattern = ParsePatternAtom();
        //    for (; ; )
        //    {
        //        switch (Tokens.Current)
        //        {
        //            case IPipeToken _:
        //                Tokens.Expect<IPipeToken>();
        //                var rightOperand = ParsePatternAtom();
        //                pattern = new OrPatternSyntax(pattern, rightOperand);
        //                break;
        //            default:
        //                return pattern;
        //        }
        //    }
        //}

        //private PatternSyntax ParsePatternAtom()
        //{
        //    switch (Tokens.Current)
        //    {
        //        case IIdentifierToken _:
        //        {
        //            var identifier = Tokens.RequiredToken<IIdentifierToken>();
        //            return new AnyPatternSyntax(identifier.Value);
        //        }
        //        case IDotToken _:
        //        {
        //            Tokens.Expect<IDotToken>();
        //            var identifier = Tokens.RequiredToken<IIdentifierToken>();
        //            return new EnumValuePatternSyntax(identifier.Value);
        //        }
        //        default:
        //            throw NonExhaustiveMatchException.For(Tokens.Current);
        //    }
        //}
    }
}
