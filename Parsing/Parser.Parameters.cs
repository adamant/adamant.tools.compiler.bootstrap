using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public INamedParameterSyntax ParseFunctionParameter()
        {
            var span = Tokens.Current.Span;
            var mutableBinding = Tokens.Accept<IVarKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierOrUnderscoreToken>();
            var name = identifier.Value;
            Tokens.Expect<IColonToken>();
            var type = ParseType();
            IExpressionSyntax? defaultValue = null;
            if (Tokens.Accept<IEqualsToken>()) defaultValue = ParseExpression();
            span = TextSpan.Covering(span, type.Span, defaultValue?.Span);
            return new NamedParameterSyntax(span, mutableBinding, name, type, defaultValue);
        }

        public IParameterSyntax ParseMethodParameter()
        {
            switch (Tokens.Current)
            {
                case IMutableKeywordToken _:
                case ISelfKeywordToken _:
                {
                    var span = Tokens.Current.Span;
                    var mutableSelf = Tokens.Accept<IMutableKeywordToken>();
                    var selfSpan = Tokens.Expect<ISelfKeywordToken>();
                    span = TextSpan.Covering(span, selfSpan);
                    return new SelfParameterSyntax(span, mutableSelf);
                }
                default:
                    return ParseFunctionParameter();
            }
        }

        public IConstructorParameterSyntax ParseConstructorParameter()
        {
            switch (Tokens.Current)
            {
                case IDotToken _:
                {
                    var dot = Tokens.Expect<IDotToken>();
                    var identifier = Tokens.RequiredToken<IIdentifierToken>();
                    var equals = Tokens.AcceptToken<IEqualsToken>();
                    IExpressionSyntax? defaultValue = null;
                    if (equals != null) defaultValue = ParseExpression();
                    var span = TextSpan.Covering(dot, identifier.Span, defaultValue?.Span);
                    Name name = identifier.Value;
                    return new FieldParameterSyntax(span, name, defaultValue);
                }
                default:
                    return ParseFunctionParameter();
            }
        }
    }
}
