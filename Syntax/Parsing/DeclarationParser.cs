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
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Function;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
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
        [NotNull] private readonly IParser<ModifierSyntax> modifierParser;

        public DeclarationParser(
            [NotNull] IListParser listParser,
            [NotNull] IParser<ExpressionSyntax> expressionParser,
            [NotNull] IParser<BlockExpressionSyntax> blockParser,
            [NotNull] IParser<ParameterSyntax> parameterParser,
            [NotNull] IParser<ModifierSyntax> modifierParser)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
            this.blockParser = blockParser;
            this.parameterParser = parameterParser;
            this.modifierParser = modifierParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public DeclarationSyntax Parse([NotNull] ITokenStream tokens, IDiagnosticsCollector diagnostics)
        {
            var modifiers = ParseModifiers(tokens, diagnostics);

            switch (tokens.Current)
            {
                case ClassKeywordToken _:
                    return ParseClass(modifiers, tokens);
                case EnumKeywordToken _:
                    return ParseEnum(modifiers, tokens, diagnostics);
                case FunctionKeywordToken _:
                    return ParseFunction(modifiers, tokens, diagnostics);
                default:
                    return ParseIncompleteDeclaration(modifiers, tokens, diagnostics);
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private SyntaxList<ModifierSyntax> ParseModifiers(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var modifiers = new List<ModifierSyntax>();
            // Take modifiers until null
            while (modifierParser.Parse(tokens, diagnostics) is ModifierSyntax modifier)
                modifiers.Add(modifier);
            return new SyntaxList<ModifierSyntax>(modifiers);
        }

        [MustUseReturnValue]
        [NotNull]
        private static DeclarationSyntax ParseClass([NotNull] SyntaxList<ModifierSyntax> modifiers, [NotNull] ITokenStream tokens)
        {
            var classKeyword = tokens.Expect<IClassKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var openBrace = tokens.Expect<IOpenBraceToken>();
            var closeBrace = tokens.Expect<ICloseBraceToken>();
            return new ClassDeclarationSyntax(modifiers, classKeyword, name, openBrace,
                SyntaxList<MemberDeclarationSyntax>.Empty, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        private static DeclarationSyntax ParseEnum([NotNull] SyntaxList<ModifierSyntax> modifiers, [NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            var enumKeyword = tokens.Expect<IEnumKeywordToken>();
            switch (tokens.Current)
            {
                case StructKeywordToken _:
                    var structKeyword = tokens.Expect<IStructKeywordToken>();
                    var name = tokens.ExpectIdentifier();
                    var openBrace = tokens.Expect<IOpenBraceToken>();
                    var closeBrace = tokens.Expect<ICloseBraceToken>();
                    return new EnumStructDeclarationSyntax(modifiers, enumKeyword, structKeyword, name,
                        openBrace, SyntaxList<MemberDeclarationSyntax>.Empty, closeBrace);
                case ClassKeywordToken _:
                    throw new NotImplementedException(
                        "Parsing enum classes not implemented");
                default:
                    return ParseIncompleteDeclaration(modifiers, tokens, diagnostics);
            }
        }

        #region Parse Function
        [MustUseReturnValue]
        [NotNull]
        public FunctionDeclarationSyntax ParseFunction(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var functionKeyword = tokens.Expect<IFunctionKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var openParen = tokens.Expect<IOpenParenToken>();
            var parameters = ParseParameters(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            var arrow = tokens.Expect<IRightArrowToken>();
            var returnTypeExpression = expressionParser.Parse(tokens, diagnostics);
            EffectsSyntax effects = null;
            if (tokens.Current is NoKeywordToken || tokens.Current is MayKeywordToken)
            {
                var mayKeyword = tokens.Expect<IMayKeywordToken>();
                var allowedEffects = listParser.ParseSeparatedList(tokens, ParseEffect, TypeOf<CommaToken>(), TypeOf<OpenBraceToken>(), diagnostics);
                var noKeyword = tokens.Expect<INoKeywordToken>();
                var disallowedEffects = listParser.ParseSeparatedList(tokens, ParseEffect, TypeOf<CommaToken>(), TypeOf<OpenBraceToken>(), diagnostics);
                effects = new EffectsSyntax(mayKeyword, allowedEffects, noKeyword, disallowedEffects);
            }
            var body = blockParser.Parse(tokens, diagnostics);
            return new FunctionDeclarationSyntax(modifiers, functionKeyword, name,
                openParen, parameters, closeParen, arrow, returnTypeExpression, effects, body);
        }

        [MustUseReturnValue]
        [NotNull]
        public EffectSyntax ParseEffect([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (tokens.Current)
            {
                case ThrowKeywordToken throwKeyword:
                    tokens.MoveNext();
                    var exceptions = listParser.ParseSeparatedList(tokens, ParseThrowEffectEntry, TypeOf<CommaToken>(), TypeOf<OpenBraceToken>(), diagnostics);
                    return new ThrowEffectSyntax(throwKeyword, exceptions);
                case IdentifierToken identifier:
                    tokens.MoveNext();
                    return new SimpleEffectSyntax(identifier);
                default:
                    throw NonExhaustiveMatchException.For(tokens.Current);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private ThrowEffectEntrySyntax ParseThrowEffectEntry([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            var paramsKeyword = tokens.Accept<ParamsKeywordToken>();
            var exceptionType = expressionParser.Parse(tokens, diagnostics);
            return new ThrowEffectEntrySyntax(paramsKeyword, exceptionType);
        }

        [MustUseReturnValue]
        [NotNull]
        private SeparatedListSyntax<ParameterSyntax> ParseParameters([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            return listParser.ParseSeparatedList(tokens, parameterParser.Parse, TypeOf<CommaToken>(), TypeOf<CloseParenToken>(), diagnostics);
        }
        #endregion

        [MustUseReturnValue]
        [NotNull]
        private static IncompleteDeclarationSyntax ParseIncompleteDeclaration(
            [NotNull] IEnumerable<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var skipped = modifiers.SelectMany(m => m.Tokens()).ToList();
            skipped.Add(tokens.ExpectIdentifier());    // The name we are expecting

            if (skipped.All(s => s is MissingToken))
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
