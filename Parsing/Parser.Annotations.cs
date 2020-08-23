using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        internal IReachabilityAnnotationsSyntax ParseReachabilityAnnotations()
        {
            var reachableFrom = AcceptReachableFromAnnotation();
            var canReach = AcceptCanReachAnnotation();
            var span = TextSpan.Covering(reachableFrom?.Span, canReach?.Span, Tokens.Current.Span.AtStart());
            return new ReachabilityAnnotationsSyntax(span, reachableFrom, canReach);
        }

        private IReachableFromAnnotationSyntax? AcceptReachableFromAnnotation()
        {
            if (!(Tokens.Current is ILeftWaveArrowToken)) return null;
            var leftArrow = Tokens.Required<ILeftWaveArrowToken>();
            var names = ParseManySeparated<INameOrSelfExpressionSyntax, ICommaToken>(ParseNameOrSelfExpression);
            var span = TextSpan.Covering(leftArrow, names[^1].Span);
            return new ReachableFromAnnotationSyntax(span, names);
        }

        private ICanReachAnnotationSyntax? AcceptCanReachAnnotation()
        {
            if (!(Tokens.Current is IRightWaveArrowToken)) return null;
            var rightArrow = Tokens.Required<IRightWaveArrowToken>();
            var names = ParseManySeparated<INameOrSelfExpressionSyntax, ICommaToken>(ParseNameOrSelfExpression);
            var span = TextSpan.Covering(rightArrow, names[^1].Span);
            return new CanReachAnnotationSyntax(span, names);
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
