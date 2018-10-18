using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
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
                //case TokenKind.MutableKeyword:
                //     `mut self`
                //    throw new NotImplementedException();
                //case TokenKind.SelfKeyword:
                //      `self`
                //    throw new NotImplementedException();
                default:
                    var varKeyword = tokens.Accept<VarKeywordToken>();
                    var name = tokens.ExpectIdentifier();
                    var colon = tokens.Expect<IColonToken>();
                    var typeExpression = expressionParser.Parse(tokens, diagnostics);
                    return new ParameterSyntax(varKeyword, name, colon, typeExpression);
            }
        }
    }
}
