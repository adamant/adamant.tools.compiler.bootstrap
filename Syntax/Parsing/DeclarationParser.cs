using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class DeclarationParser : IParser<DeclarationSyntax>
    {
        [NotNull]
        private readonly IListParser listParser;

        [NotNull]
        private readonly IParser<ExpressionSyntax> expressionParser;

        [NotNull]
        private readonly IParser<BlockStatementSyntax> blockStatementParser;

        [NotNull]
        private readonly IParser<ParameterSyntax> parameterParser;

        public DeclarationParser(
            [NotNull] IListParser listParser,
            [NotNull] IParser<ExpressionSyntax> expressionParser,
            [NotNull] IParser<BlockStatementSyntax> blockStatementParser,
            [NotNull] IParser<ParameterSyntax> parameterParser)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
            this.blockStatementParser = blockStatementParser;
            this.parameterParser = parameterParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public DeclarationSyntax Parse([NotNull] ITokenStream tokens)
        {
            var accessModifier = ParseAccessModifier(tokens);

            switch (tokens.Current)
            {
                case ClassKeywordToken _:
                    return ParseClass(accessModifier, tokens);
                case EnumKeywordToken _:
                    return ParseEnum(accessModifier, tokens);
                case FunctionKeywordToken _:
                    return ParseFunction(accessModifier, tokens);
                default:
                    return ParseIncompleteDeclaration(tokens, accessModifier);
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
            }
        }

        [MustUseReturnValue]
        [CanBeNull]
        private static KeywordToken ParseAccessModifier([NotNull] ITokenStream tokens)
        {
            switch (tokens.Current)
            {
                case PublicKeywordToken _:
                case ProtectedKeywordToken _:
                case PrivateKeywordToken _:
                    return tokens.ExpectKeyword();
                default:
                    return tokens.MissingToken<KeywordToken>();
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private static DeclarationSyntax ParseClass([CanBeNull] KeywordToken accessModifier, [NotNull] ITokenStream tokens)
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
        private static DeclarationSyntax ParseEnum([CanBeNull] KeywordToken accessModifier, [NotNull] ITokenStream tokens)
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
                    return ParseIncompleteDeclaration(tokens, accessModifier);
            }
        }

        #region Parse Function
        [MustUseReturnValue]
        [NotNull]
        public FunctionDeclarationSyntax ParseFunction(
            [CanBeNull] KeywordToken accessModifier,
            [NotNull] ITokenStream tokens)
        {
            var functionKeyword = tokens.Expect<FunctionKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var openParen = tokens.Expect<OpenParenToken>();
            var parameters = ParseParameters(tokens);
            var closeParen = tokens.Expect<CloseParenToken>();
            var arrow = tokens.Expect<RightArrowToken>();
            var returnTypeExpression = expressionParser.Parse(tokens);
            var body = blockStatementParser.Parse(tokens);
            return new FunctionDeclarationSyntax(accessModifier, functionKeyword, name,
                openParen, parameters, closeParen, arrow, returnTypeExpression, body);
        }

        [MustUseReturnValue]
        [NotNull]
        private SeparatedListSyntax<ParameterSyntax> ParseParameters([NotNull] ITokenStream tokens)
        {
            return listParser.ParseSeparatedList(tokens, parameterParser.Parse, TypeOf<CommaToken>._, TypeOf<CloseParenToken>._);
        }
        #endregion

        [MustUseReturnValue]
        [NotNull]
        private static IncompleteDeclarationSyntax ParseIncompleteDeclaration([NotNull] ITokenStream tokens, [CanBeNull] KeywordToken accessModifier)
        {
            var skipped = new List<ISyntaxNodeOrToken>() { accessModifier };
            var startToken = tokens.Current;
            var name = tokens.ExpectIdentifier();
            skipped.Add(name);
            throw new NotImplementedException();
            //if (tokens.Current == start)
            //    // We have not advanced at all when trying to parse a declaration.
            //    // Skip a token to try to see if we can find the start of a declaration.
            //    children.Add(new SkippedTokensSyntax(tokens.ConsumeAny()));
            //EnsureAdvance(tokens, startToken, children);

            //return new IncompleteDeclarationSyntax(accessModifier, name, );
        }
    }
}
