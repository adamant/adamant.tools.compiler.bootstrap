using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Errors;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class DeclarationParser : IParser<DeclarationSyntax>
    {
        [NotNull] private readonly IListParser listParser;
        [NotNull] private readonly IParser<ExpressionSyntax> expressionParser;
        [NotNull] private readonly IParser<BlockExpressionSyntax> blockParser;
        [NotNull] private readonly IParser<ParameterSyntax> parameterParser;
        [NotNull] private readonly IParser<AccessModifierSyntax> accessModifierParser;

        public DeclarationParser(
            [NotNull] IListParser listParser,
            [NotNull] IParser<ExpressionSyntax> expressionParser,
            [NotNull] IParser<BlockExpressionSyntax> blockParser,
            [NotNull] IParser<ParameterSyntax> parameterParser,
            [NotNull] IParser<AccessModifierSyntax> accessModifierParser)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
            this.blockParser = blockParser;
            this.parameterParser = parameterParser;
            this.accessModifierParser = accessModifierParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public DeclarationSyntax Parse([NotNull] ITokenStream tokens, IDiagnosticsCollector diagnostics)
        {
            var accessModifier = accessModifierParser.Parse(tokens, diagnostics);

            switch (tokens.Current)
            {
                case ClassKeywordToken _:
                    return ParseClass(accessModifier, tokens);
                case EnumKeywordToken _:
                    return ParseEnum(accessModifier, tokens, diagnostics);
                case FunctionKeywordToken _:
                    return ParseFunction(accessModifier, tokens, diagnostics);
                default:
                    return ParseIncompleteDeclaration(accessModifier, tokens, diagnostics);
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private static DeclarationSyntax ParseClass([NotNull] AccessModifierSyntax accessModifier, [NotNull] ITokenStream tokens)
        {
            var classKeyword = tokens.Expect<ClassKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var openBrace = tokens.Expect<OpenBraceToken>();
            var closeBrace = tokens.Expect<CloseBraceToken>();
            return new ClassDeclarationSyntax(accessModifier, classKeyword, name, openBrace,
                SyntaxList<MemberDeclarationSyntax>.Empty, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        private static DeclarationSyntax ParseEnum([NotNull] AccessModifierSyntax accessModifier, [NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            var enumKeyword = tokens.Expect<EnumKeywordToken>();
            switch (tokens.Current)
            {
                case StructKeywordToken _:
                    var structKeyword = tokens.Expect<StructKeywordToken>();
                    var name = tokens.ExpectIdentifier();
                    var openBrace = tokens.Expect<OpenBraceToken>();
                    var closeBrace = tokens.Expect<CloseBraceToken>();
                    return new EnumStructDeclarationSyntax(accessModifier, enumKeyword, structKeyword, name,
                        openBrace, SyntaxList<MemberDeclarationSyntax>.Empty, closeBrace);
                case ClassKeywordToken _:
                    throw new NotImplementedException(
                        "Parsing enum classes not implemented");
                default:
                    return ParseIncompleteDeclaration(accessModifier, tokens, diagnostics);
            }
        }

        #region Parse Function
        [MustUseReturnValue]
        [NotNull]
        public FunctionDeclarationSyntax ParseFunction(
            [NotNull] AccessModifierSyntax accessModifier,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var functionKeyword = tokens.Expect<FunctionKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var openParen = tokens.Expect<OpenParenToken>();
            var parameters = ParseParameters(tokens, diagnostics);
            var closeParen = tokens.Expect<CloseParenToken>();
            var arrow = tokens.Expect<RightArrowToken>();
            var returnTypeExpression = expressionParser.Parse(tokens, diagnostics);
            var body = blockParser.Parse(tokens, diagnostics);
            return new FunctionDeclarationSyntax(accessModifier, functionKeyword, name,
                openParen, parameters, closeParen, arrow, returnTypeExpression, body);
        }

        [MustUseReturnValue]
        [NotNull]
        private SeparatedListSyntax<ParameterSyntax> ParseParameters([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            return listParser.ParseSeparatedList(tokens, t => parameterParser.Parse(t, diagnostics), TypeOf<CommaToken>(), TypeOf<CloseParenToken>(), diagnostics);
        }
        #endregion

        [MustUseReturnValue]
        [NotNull]
        private static IncompleteDeclarationSyntax ParseIncompleteDeclaration(
            [CanBeNull] AccessModifierSyntax accessModifier,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var skipped = new List<Token>
            {
                accessModifier?.Keyword,
                tokens.ExpectIdentifier() // The name we are expecting
            };

            if (skipped.All(s => s == null))
            {
                // We haven't consumed any tokens, we need to consume a token so
                // we make progress.
                skipped.Add(tokens.Current);
                tokens.MoveNext();
            }

            var span = TextSpan.Covering(
                skipped.First(s => s != null).AssertNotNull().Span,
                skipped.Last(s => s != null).AssertNotNull().Span);

            diagnostics.Publish(ParseError.IncompleteDeclaration(tokens.File, span));
            return new IncompleteDeclarationSyntax(skipped);
        }
    }
}
