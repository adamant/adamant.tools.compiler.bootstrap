using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        internal FixedList<IReachabilityAnnotationSyntax> ParseReachabilityAnnotations()
        {
            return AcceptMany(AcceptReachabilityAnnotation);
        }

        private IReachabilityAnnotationSyntax? AcceptReachabilityAnnotation()
        {
            switch (Tokens.Current)
            {
                case IRightWaveArrowToken _:
                {
                    var rightArrow = Tokens.Required<IRightWaveArrowToken>();
                    var names = ParseManySeparated<INameOrSelfExpressionSyntax, ICommaToken>(ParseNameOrSelfExpression);
                    var span = TextSpan.Covering(rightArrow, names[^1].Span);
                    return new CanReachAnnotationSyntax(span, names);
                }
                case ILeftWaveArrowToken _:
                {
                    var leftArrow = Tokens.Required<ILeftWaveArrowToken>();
                    var names = ParseManySeparated<INameOrSelfExpressionSyntax, ICommaToken>(ParseNameOrSelfExpression);
                    var span = TextSpan.Covering(leftArrow, names[^1].Span);
                    return new ReachableFromAnnotationSyntax(span, names);
                }
                default:
                    return null;
            }
        }

        private INameOrSelfExpressionSyntax ParseNameOrSelfExpression()
        {
            return Tokens.Current switch
            {
                ISelfKeywordToken _ => ParseSelfExpression(),
                IIdentifierToken _ => ParseName().ToExpression(),
                _ => ParseMissingIdentifier()
            };
        }
    }
}
