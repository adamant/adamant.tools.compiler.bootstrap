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
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Field;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Contracts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Effects;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Parameters;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types.Inheritance;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class DeclarationParser : IParser<DeclarationSyntax>
    {
        [NotNull] private readonly IListParser listParser;
        [NotNull] private readonly IExpressionParser expressionParser;
        [NotNull] private readonly IParser<BlockExpressionSyntax> blockParser;
        [NotNull] private readonly IParser<ParameterSyntax> parameterParser;
        [NotNull] private readonly IParser<ModifierSyntax> modifierParser;

        public DeclarationParser(
            [NotNull] IListParser listParser,
            [NotNull] IExpressionParser expressionParser,
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
        public DeclarationSyntax Parse(
            [NotNull] ITokenStream tokens,
            IDiagnosticsCollector diagnostics)
        {
            var modifiers = ParseModifiers(tokens, diagnostics);

            switch (tokens.Current)
            {
                case ClassKeywordToken _:
                    return ParseClass(modifiers, tokens, diagnostics);
                case TypeKeywordToken _:
                    return ParseType(modifiers, tokens, diagnostics);
                case StructKeywordToken _:
                    return ParseStruct(modifiers, tokens, diagnostics);
                case EnumKeywordToken _:
                    return ParseEnum(modifiers, tokens, diagnostics);
                case FunctionKeywordToken _:
                    return ParseNamedFunction(modifiers, tokens, diagnostics);
                default:
                    return ParseIncompleteDeclaration(modifiers, tokens, diagnostics);
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
            }
        }

        [MustUseReturnValue]
        [NotNull]
        public MemberDeclarationSyntax ParseMemberDeclaration(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var modifiers = ParseModifiers(tokens, diagnostics);

            switch (tokens.Current)
            {
                case ClassKeywordToken _:
                    return ParseClass(modifiers, tokens, diagnostics);
                case TypeKeywordToken _:
                    return ParseType(modifiers, tokens, diagnostics);
                case StructKeywordToken _:
                    return ParseStruct(modifiers, tokens, diagnostics);
                case EnumKeywordToken _:
                    return ParseEnum(modifiers, tokens, diagnostics);
                case FunctionKeywordToken _:
                    return ParseNamedFunction(modifiers, tokens, diagnostics);
                case GetKeywordToken _:
                    return ParseGetterFunction(modifiers, tokens, diagnostics);
                case SetKeywordToken _:
                    return ParseSetterFunction(modifiers, tokens, diagnostics);
                case VarKeywordToken _:
                case LetKeywordToken _:
                    return ParseField(modifiers, tokens, diagnostics);
                case NewKeywordToken _:
                    return ParseConstructor(modifiers, tokens, diagnostics);
                default:
                    return ParseIncompleteDeclaration(modifiers, tokens, diagnostics);
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
            }
        }

        #region Parse Type Parts
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
        [CanBeNull]
        private GenericParametersSyntax ParseGenericParameters(
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
        private GenericParameterSyntax ParseGenericParameter(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var paramsKeyword = tokens.Accept<ParamsKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var colon = tokens.Accept<ColonToken>();
            var typeExpression = colon != null ? expressionParser.Parse(tokens, diagnostics) : null;
            return new GenericParameterSyntax(paramsKeyword, name, colon, typeExpression);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private BaseClassSyntax ParseBaseClass(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var colon = tokens.Accept<ColonToken>();
            if (colon == null) return null;
            var typeExpression = expressionParser.Parse(tokens, diagnostics);
            return new BaseClassSyntax(colon, typeExpression);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private BaseTypesSyntax ParseBaseTypes(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var lessThanColon = tokens.Accept<LessThanColonToken>();
            if (lessThanColon == null) return null;
            var typeExpressions = listParser.ParseSeparatedList(tokens, expressionParser.Parse,
                TypeOf<CommaToken>(), TypeOf<OpenBraceToken>(), diagnostics);
            return new BaseTypesSyntax(lessThanColon, typeExpressions);
        }
        #endregion

        #region Parse Type Declarations
        [MustUseReturnValue]
        [NotNull]
        private ClassDeclarationSyntax ParseClass(
          [NotNull] SyntaxList<ModifierSyntax> modifiers,
          [NotNull] ITokenStream tokens,
          [NotNull] IDiagnosticsCollector diagnostics)
        {
            var classKeyword = tokens.Take<ClassKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var genericParameters = ParseGenericParameters(tokens, diagnostics);
            var baseClass = ParseBaseClass(tokens, diagnostics);
            var baseTypes = ParseBaseTypes(tokens, diagnostics);
            var openBrace = tokens.Expect<IOpenBraceToken>();
            var members = listParser.ParseList(tokens, ParseMemberDeclaration, TypeOf<CloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceToken>();
            return new ClassDeclarationSyntax(modifiers, classKeyword, name, genericParameters, baseClass, baseTypes, openBrace,
                members, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        private TypeDeclarationSyntax ParseType(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var typeKeyword = tokens.Take<TypeKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var genericParameters = ParseGenericParameters(tokens, diagnostics);
            var baseTypes = ParseBaseTypes(tokens, diagnostics);
            var openBrace = tokens.Expect<IOpenBraceToken>();
            var members = listParser.ParseList(tokens, ParseMemberDeclaration, TypeOf<CloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceToken>();
            return new TypeDeclarationSyntax(modifiers, typeKeyword, name, genericParameters, baseTypes, openBrace,
                members, closeBrace);
        }


        [MustUseReturnValue]
        [NotNull]
        private StructDeclarationSyntax ParseStruct(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var structKeyword = tokens.Take<StructKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var genericParameters = ParseGenericParameters(tokens, diagnostics);
            var baseTypes = ParseBaseTypes(tokens, diagnostics);
            var openBrace = tokens.Expect<IOpenBraceToken>();
            var members = listParser.ParseList(tokens, ParseMemberDeclaration, TypeOf<CloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceToken>();
            return new StructDeclarationSyntax(modifiers, structKeyword, name, genericParameters, baseTypes, openBrace,
                members, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        private MemberDeclarationSyntax ParseEnum(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var enumKeyword = tokens.Take<EnumKeywordToken>();
            switch (tokens.Current)
            {
                case StructKeywordToken _:
                    var structKeyword = tokens.Expect<IStructKeywordToken>();
                    var name = tokens.ExpectIdentifier();
                    var openBrace = tokens.Expect<IOpenBraceToken>();
                    var members = listParser.ParseList(tokens, ParseMemberDeclaration, TypeOf<CloseBraceToken>(), diagnostics);
                    var closeBrace = tokens.Expect<ICloseBraceToken>();
                    return new EnumStructDeclarationSyntax(modifiers, enumKeyword, structKeyword, name,
                        openBrace, members, closeBrace);
                case ClassKeywordToken _:
                    throw new NotImplementedException(
                        "Parsing enum classes not implemented");
                default:
                    return ParseIncompleteDeclaration(modifiers, tokens, diagnostics);
            }
        }
        #endregion

        #region Parse Type Member Declarations
        [MustUseReturnValue]
        [NotNull]
        private FieldDeclarationSyntax ParseField(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var binding = tokens.Take<IBindingKeywordToken>();
            var getter = ParseFieldGetter(tokens, diagnostics);
            var name = tokens.ExpectIdentifier();
            IColonToken colon = null;
            ExpressionSyntax typeExpression = null;
            if (tokens.Current is ColonToken)
            {
                colon = tokens.Expect<IColonToken>();
                // Need to not consume the assignment that separates the type from the initializer,
                // hence the min operator precedence.
                typeExpression = expressionParser.Parse(tokens, diagnostics, OperatorPrecedence.LogicalOr);
            }
            EqualsToken equals = null;
            ExpressionSyntax initializer = null;
            if (tokens.Current is EqualsToken)
            {
                equals = tokens.Take<EqualsToken>();
                initializer = expressionParser.Parse(tokens, diagnostics);
            }
            var semicolon = tokens.Expect<ISemicolonToken>();
            return new FieldDeclarationSyntax(modifiers, binding, getter, name, colon, typeExpression,
                equals, initializer, semicolon);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private static FieldGetterSyntax ParseFieldGetter(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var publicKeyword = tokens.Accept<PublicKeywordToken>();
            var getKeyword = tokens.Accept<GetKeywordToken>();
            if (publicKeyword == null && getKeyword == null) return null;
            return new FieldGetterSyntax(publicKeyword, getKeyword);
        }
        #endregion

        #region Parse Functions
        [MustUseReturnValue]
        [NotNull]
        public NamedFunctionDeclarationSyntax ParseNamedFunction(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var functionKeyword = tokens.Take<FunctionKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var genericParameters = ParseGenericParameters(tokens, diagnostics);
            var openParen = tokens.Expect<IOpenParenToken>();
            var parameters = ParseParameters(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            var arrow = tokens.Expect<IRightArrowToken>();
            var returnTypeExpression = expressionParser.Parse(tokens, diagnostics);
            var effects = ParseEffects(tokens, diagnostics);
            var contracts = ParseContracts(tokens, diagnostics);
            var body = blockParser.Parse(tokens, diagnostics);
            return new NamedFunctionDeclarationSyntax(modifiers, functionKeyword, name, genericParameters,
                openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body);
        }

        [MustUseReturnValue]
        [NotNull]
        public ConstructorFunctionDeclarationSyntax ParseConstructor(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var newKeyword = tokens.Take<NewKeywordToken>();
            var name = tokens.Accept<IdentifierToken>();
            var genericParameters = ParseGenericParameters(tokens, diagnostics);
            var openParen = tokens.Expect<IOpenParenToken>();
            var parameters = ParseParameters(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            var effects = ParseEffects(tokens, diagnostics);
            var contracts = ParseContracts(tokens, diagnostics);
            var body = blockParser.Parse(tokens, diagnostics);
            return new ConstructorFunctionDeclarationSyntax(modifiers, newKeyword, name,
                genericParameters, openParen, parameters, closeParen, effects, contracts, body);
        }

        [MustUseReturnValue]
        [NotNull]
        private GetterFunctionDeclarationSyntax ParseGetterFunction(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var getKeyword = tokens.Take<GetKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var openParen = tokens.Expect<IOpenParenToken>();
            var parameters = ParseParameters(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            var arrow = tokens.Expect<IRightArrowToken>();
            var returnTypeExpression = expressionParser.Parse(tokens, diagnostics);
            var effects = ParseEffects(tokens, diagnostics);
            var contracts = ParseContracts(tokens, diagnostics);
            var body = blockParser.Parse(tokens, diagnostics);
            return new GetterFunctionDeclarationSyntax(modifiers, getKeyword, name,
                openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body);
        }

        [MustUseReturnValue]
        [NotNull]
        private SetterFunctionDeclarationSyntax ParseSetterFunction(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var setKeyword = tokens.Take<SetKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var openParen = tokens.Expect<IOpenParenToken>();
            var parameters = ParseParameters(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            var arrow = tokens.Expect<IRightArrowToken>();
            var returnTypeExpression = expressionParser.Parse(tokens, diagnostics);
            var effects = ParseEffects(tokens, diagnostics);
            var contracts = ParseContracts(tokens, diagnostics);
            var body = blockParser.Parse(tokens, diagnostics);
            return new SetterFunctionDeclarationSyntax(modifiers, setKeyword, name,
                openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body);
        }

        [MustUseReturnValue]
        [NotNull]
        private SeparatedListSyntax<ParameterSyntax> ParseParameters([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            return listParser.ParseSeparatedList(tokens, parameterParser.Parse, TypeOf<CommaToken>(), TypeOf<CloseParenToken>(), diagnostics);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private EffectsSyntax ParseEffects(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            if (!(tokens.Current is NoKeywordToken) && !(tokens.Current is MayKeywordToken))
                return null;

            var mayKeyword = tokens.Expect<IMayKeywordToken>();
            var allowedEffects = listParser.ParseSeparatedList(tokens, ParseEffect,
                TypeOf<CommaToken>(), TypeOf<OpenBraceToken>(), diagnostics);
            var noKeyword = tokens.Expect<INoKeywordToken>();
            var disallowedEffects = listParser.ParseSeparatedList(tokens, ParseEffect,
                TypeOf<CommaToken>(), TypeOf<OpenBraceToken>(), diagnostics);
            return new EffectsSyntax(mayKeyword, allowedEffects, noKeyword, disallowedEffects);
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
        private SyntaxList<ContractSyntax> ParseContracts(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var contracts = new List<ContractSyntax>();
            for (; ; )
                switch (tokens.Current)
                {
                    case RequiresKeywordToken _:
                        contracts.Add(ParseRequires(tokens, diagnostics));
                        break;
                    case EnsuresKeywordToken _:
                        contracts.Add(ParseEnsures(tokens, diagnostics));
                        break;
                    default:
                        return contracts.ToSyntaxList();
                }
        }

        [MustUseReturnValue]
        [NotNull]
        private RequiresSyntax ParseRequires(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var requiresKeyword = tokens.Take<RequiresKeywordToken>();
            var condition = expressionParser.Parse(tokens, diagnostics);
            return new RequiresSyntax(requiresKeyword, condition);
        }

        [MustUseReturnValue]
        [NotNull]
        private EnsuresSyntax ParseEnsures(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var ensuresKeyword = tokens.Take<EnsuresKeywordToken>();
            var condition = expressionParser.Parse(tokens, diagnostics);
            return new EnsuresSyntax(ensuresKeyword, condition);
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
