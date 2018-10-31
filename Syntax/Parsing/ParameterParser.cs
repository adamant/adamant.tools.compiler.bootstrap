using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Parameters;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;
using VarKeywordToken = Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.VarKeywordToken;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class ParameterParser : IParameterParser
    {
        [NotNull] private readonly IExpressionParser expressionParser;

        public ParameterParser([NotNull] IExpressionParser expressionParser)
        {
            this.expressionParser = expressionParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public ParameterSyntax ParseParameter([NotNull] ITokenStream tokens, IDiagnosticsCollector diagnostics)
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
                    var typeExpression = expressionParser.ParseExpression(tokens, diagnostics);
                    return new NamedParameterSyntax(paramsKeyword, varKeyword, name, colon, typeExpression);
            }
        }
    }
}
