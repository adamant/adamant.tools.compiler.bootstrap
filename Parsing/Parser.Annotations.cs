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
            var parameters = ParseManySeparated<IParameterNameSyntax, ICommaToken>(ParseParameterName);
            var span = TextSpan.Covering(leftArrow, parameters[^1].Span);
            return new ReachableFromAnnotationSyntax(span, parameters);
        }

        private ICanReachAnnotationSyntax? AcceptCanReachAnnotation()
        {
            if (!(Tokens.Current is IRightWaveArrowToken)) return null;
            var rightArrow = Tokens.Required<IRightWaveArrowToken>();
            var parameters = ParseManySeparated<IParameterNameSyntax, ICommaToken>(ParseParameterName);
            var span = TextSpan.Covering(rightArrow, parameters[^1].Span);
            return new CanReachAnnotationSyntax(span, parameters);
        }

        private IParameterNameSyntax ParseParameterName()
        {

            switch (Tokens.Current)
            {
                case ISelfKeywordToken _:
                {
                    var selfKeyword = Tokens.Required<ISelfKeywordToken>();
                    return new SelfParameterNameSyntax(selfKeyword);
                }
                case IIdentifierToken _:
                {
                    var identifier = Tokens.RequiredToken<IIdentifierToken>();
                    return new NamedParameterNameSyntax(identifier.Span, identifier.Value);
                }
                default:
                {
                    var identifierSpan = Tokens.Expect<IIdentifierToken>();
                    return new NamedParameterNameSyntax(identifierSpan, null);
                }
            }
        }
    }
}
