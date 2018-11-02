using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class GenericsParser : IGenericsParser
    {
        [NotNull] private readonly IListParser listParser;
        [NotNull] private readonly IExpressionParser expressionParser;

        public GenericsParser(
            [NotNull] IListParser listParser,
            [NotNull] IExpressionParser expressionParser)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
        }

        [MustUseReturnValue]
        [CanBeNull]
        public GenericParametersSyntax AcceptGenericParameters(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var openBracket = tokens.Accept<OpenBracketToken>();
            if (openBracket == null) return null;
            var parameters = listParser.ParseSeparatedList(tokens, ParseGenericParameter,
                TypeOf<CommaToken>(), TypeOf<CloseBracketToken>(), diagnostics);
            var closeBracket = tokens.Expect<ICloseBracketToken>();
            return new GenericParametersSyntax(openBracket, parameters, closeBracket);
        }

        [MustUseReturnValue]
        [NotNull]
        public GenericParameterSyntax ParseGenericParameter(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var paramsKeyword = tokens.Accept<ParamsKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var colon = tokens.Accept<ColonToken>();
            var typeExpression = colon != null ? expressionParser.ParseExpression(tokens, diagnostics) : null;
            return new GenericParameterSyntax(paramsKeyword, name, colon, typeExpression);
        }

        [MustUseReturnValue]
        [NotNull]
        public SyntaxList<GenericConstraintSyntax> ParseGenericConstraints(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var constraints = new List<GenericConstraintSyntax>();
            while (tokens.Current is WhereKeywordToken)
                constraints.Add(ParseGenericConstraint(tokens, diagnostics));

            return constraints.ToSyntaxList();
        }

        [MustUseReturnValue]
        [NotNull]
        public GenericConstraintSyntax ParseGenericConstraint(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var whereKeyword = tokens.Take<WhereKeywordToken>();
            var expression = expressionParser.ParseExpression(tokens, diagnostics);
            return new GenericConstraintSyntax(whereKeyword, expression);
        }
    }
}
