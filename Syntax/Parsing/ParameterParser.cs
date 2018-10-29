using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Function;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;
using VarKeywordToken = Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.VarKeywordToken;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class ParameterParser : IParser<ParameterSyntax>
    {
        [NotNull]
        private readonly IParser<ExpressionSyntax> expressionParser;

        public ParameterParser([NotNull] IParser<ExpressionSyntax> expressionParser)
        {
            this.expressionParser = expressionParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public ParameterSyntax Parse([NotNull] ITokenStream tokens, IDiagnosticsCollector diagnostics)
        {
            switch (tokens.Current)
            {
                case MutableKeywordToken mutableKeyword:
                    {
                        tokens.MoveNext();
                        var selfKeyword = tokens.Expect<SelfKeywordToken>();
                        return new SelfParameterSyntax(mutableKeyword, selfKeyword);
                    }
                case SelfKeywordToken selfKeyword:
                    tokens.MoveNext();
                    return new SelfParameterSyntax(null, selfKeyword);
                default:
                    var paramsKeyword = tokens.Accept<ParamsKeywordToken>();
                    var varKeyword = tokens.Accept<VarKeywordToken>();
                    var name = tokens.ExpectIdentifier();
                    var colon = tokens.Expect<IColonToken>();
                    var typeExpression = expressionParser.Parse(tokens, diagnostics);
                    return new NamedParameterSyntax(paramsKeyword, varKeyword, name, colon, typeExpression);
            }
        }
    }
}
