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
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types.Inheritance;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Blocks;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class DeclarationParser : IDeclarationParser
    {
        [NotNull] private readonly IListParser listParser;
        [NotNull] private readonly IExpressionParser expressionParser;
        [NotNull] private readonly IBlockParser blockParser;
        [NotNull] private readonly IParameterParser parameterParser;
        [NotNull] private readonly IModifierParser modifierParser;
        [NotNull] private readonly IGenericsParser genericsParser;

        public DeclarationParser(
            [NotNull] IListParser listParser,
            [NotNull] IExpressionParser expressionParser,
            [NotNull] IBlockParser blockParser,
            [NotNull] IParameterParser parameterParser,
            [NotNull] IModifierParser modifierParser,
            [NotNull] IGenericsParser genericsParser)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
            this.blockParser = blockParser;
            this.parameterParser = parameterParser;
            this.modifierParser = modifierParser;
            this.genericsParser = genericsParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public DeclarationSyntax ParseDeclaration(
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
                case ConstKeywordToken _:
                    return ParseConst(modifiers, tokens, diagnostics);
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
                case OperatorKeywordToken _:
                    return ParseOperatorFunction(modifiers, tokens, diagnostics);
                case NewKeywordToken _:
                    return ParseConstructor(modifiers, tokens, diagnostics);
                case DeleteKeywordToken _:
                    return ParseDestructor(modifiers, tokens, diagnostics);
                case GetKeywordToken _:
                    return ParseGetterFunction(modifiers, tokens, diagnostics);
                case SetKeywordToken _:
                    return ParseSetterFunction(modifiers, tokens, diagnostics);
                case VarKeywordToken _:
                case LetKeywordToken _:
                    return ParseField(modifiers, tokens, diagnostics);
                case ConstKeywordToken _:
                    return ParseConst(modifiers, tokens, diagnostics);
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
            while (modifierParser.AcceptModifier(tokens, diagnostics) is ModifierSyntax modifier)
                modifiers.Add(modifier);
            return new SyntaxList<ModifierSyntax>(modifiers);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private BaseClassSyntax AcceptBaseClass(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var colon = tokens.Accept<ColonToken>();
            if (colon == null) return null;
            var typeExpression = expressionParser.ParseExpression(tokens, diagnostics);
            return new BaseClassSyntax(colon, typeExpression);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private BaseTypesSyntax AcceptBaseTypes(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var lessThanColon = tokens.Accept<LessThanColonToken>();
            if (lessThanColon == null) return null;
            var typeExpressions = listParser.ParseSeparatedList(tokens, expressionParser.ParseExpression,
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
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
            var baseClass = AcceptBaseClass(tokens, diagnostics);
            var baseTypes = AcceptBaseTypes(tokens, diagnostics);
            var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
            var openBrace = tokens.Expect<IOpenBraceToken>();
            var members = listParser.ParseList(tokens, ParseMemberDeclaration, TypeOf<CloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceToken>();
            return new ClassDeclarationSyntax(modifiers, classKeyword, name, genericParameters, baseClass, baseTypes, genericConstraints, openBrace,
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
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
            var baseTypes = AcceptBaseTypes(tokens, diagnostics);
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
            var name = tokens.Expect<IIdentifierOrPrimitiveToken>();
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
            var baseTypes = AcceptBaseTypes(tokens, diagnostics);
            var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
            var openBrace = tokens.Expect<IOpenBraceToken>();
            var members = listParser.ParseList(tokens, ParseMemberDeclaration, TypeOf<CloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceToken>();
            return new StructDeclarationSyntax(modifiers, structKeyword, name, genericParameters, baseTypes, genericConstraints, openBrace,
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
            var getter = AcceptFieldGetter(tokens, diagnostics);
            var name = tokens.ExpectIdentifier();
            IColonToken colon = null;
            ExpressionSyntax typeExpression = null;
            if (tokens.Current is ColonToken)
            {
                colon = tokens.Expect<IColonToken>();
                // Need to not consume the assignment that separates the type from the initializer,
                // hence the min operator precedence.
                typeExpression = expressionParser.ParseExpression(tokens, diagnostics, OperatorPrecedence.LogicalOr);
            }
            EqualsToken equals = null;
            ExpressionSyntax initializer = null;
            if (tokens.Current is EqualsToken)
            {
                equals = tokens.Take<EqualsToken>();
                initializer = expressionParser.ParseExpression(tokens, diagnostics);
            }
            var semicolon = tokens.Expect<ISemicolonToken>();
            return new FieldDeclarationSyntax(modifiers, binding, getter, name, colon, typeExpression,
                equals, initializer, semicolon);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private static FieldGetterSyntax AcceptFieldGetter(
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
        private (BlockSyntax, ISemicolonToken) ParseFunctionBody(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var body = blockParser.AcceptBlock(tokens, diagnostics);
            ISemicolonToken semicolon = null;
            if (body == null)
                semicolon = tokens.Expect<ISemicolonToken>();
            return (body, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        public NamedFunctionDeclarationSyntax ParseNamedFunction(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var functionKeyword = tokens.Take<FunctionKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
            var openParen = tokens.Expect<IOpenParenToken>();
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            var arrow = tokens.Expect<IRightArrowToken>();
            var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseContracts(tokens, diagnostics);
            var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
            return new NamedFunctionDeclarationSyntax(modifiers, functionKeyword, name, genericParameters,
                openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        public OperatorFunctionDeclarationSyntax ParseOperatorFunction(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var operatorKeyword = tokens.Take<OperatorKeywordToken>();
            var @operator = tokens.Expect<IOperatorToken>();
            var openParen = tokens.Expect<IOpenParenToken>();
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            var arrow = tokens.Expect<IRightArrowToken>();
            var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseContracts(tokens, diagnostics);
            var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
            return new OperatorFunctionDeclarationSyntax(modifiers, operatorKeyword, @operator,
                openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body, semicolon);
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
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
            var openParen = tokens.Expect<IOpenParenToken>();
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseContracts(tokens, diagnostics);
            var body = blockParser.ParseBlock(tokens, diagnostics);
            return new ConstructorFunctionDeclarationSyntax(modifiers, newKeyword, name,
                genericParameters, openParen, parameters, closeParen, effects, contracts, body);
        }

        [MustUseReturnValue]
        [NotNull]
        public DestructorFunctionDeclarationSyntax ParseDestructor(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var deleteKeyword = tokens.Take<DeleteKeywordToken>();
            var openParen = tokens.Expect<IOpenParenToken>();
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseContracts(tokens, diagnostics);
            var body = blockParser.ParseBlock(tokens, diagnostics);
            return new DestructorFunctionDeclarationSyntax(modifiers, deleteKeyword,
                 openParen, parameters, closeParen, effects, contracts, body);
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
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            var arrow = tokens.Expect<IRightArrowToken>();
            var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseContracts(tokens, diagnostics);
            var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
            return new GetterFunctionDeclarationSyntax(modifiers, getKeyword, name,
                openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body, semicolon);
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
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            var arrow = tokens.Expect<IRightArrowToken>();
            var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseContracts(tokens, diagnostics);
            var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
            return new SetterFunctionDeclarationSyntax(modifiers, setKeyword, name,
                openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        private SeparatedListSyntax<ParameterSyntax> ParseParameterList(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            return listParser.ParseSeparatedList(tokens, parameterParser.ParseParameter, TypeOf<CommaToken>(), TypeOf<CloseParenToken>(), diagnostics);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private EffectsSyntax AcceptEffects(
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
        public EffectSyntax ParseEffect(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
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
        private ThrowEffectEntrySyntax ParseThrowEffectEntry(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var paramsKeyword = tokens.Accept<ParamsKeywordToken>();
            var exceptionType = expressionParser.ParseExpression(tokens, diagnostics);
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
            var condition = expressionParser.ParseExpression(tokens, diagnostics);
            return new RequiresSyntax(requiresKeyword, condition);
        }

        [MustUseReturnValue]
        [NotNull]
        private EnsuresSyntax ParseEnsures(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var ensuresKeyword = tokens.Take<EnsuresKeywordToken>();
            var condition = expressionParser.ParseExpression(tokens, diagnostics);
            return new EnsuresSyntax(ensuresKeyword, condition);
        }
        #endregion

        [MustUseReturnValue]
        [NotNull]
        private ConstDeclarationSyntax ParseConst(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var constKeyword = tokens.Take<ConstKeywordToken>();
            var name = tokens.ExpectIdentifier();
            IColonToken colon = null;
            ExpressionSyntax typeExpression = null;
            if (tokens.Current is ColonToken)
            {
                colon = tokens.Expect<IColonToken>();
                // Need to not consume the assignment that separates the type from the initializer,
                // hence the min operator precedence.
                typeExpression = expressionParser.ParseExpression(tokens, diagnostics, OperatorPrecedence.LogicalOr);
            }
            EqualsToken equals = null;
            ExpressionSyntax initializer = null;
            if (tokens.Current is EqualsToken)
            {
                equals = tokens.Take<EqualsToken>();
                initializer = expressionParser.ParseExpression(tokens, diagnostics);
            }
            var semicolon = tokens.Expect<ISemicolonToken>();
            return new ConstDeclarationSyntax(modifiers, constKeyword, name, colon, typeExpression,
                equals, initializer, semicolon);
        }

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
