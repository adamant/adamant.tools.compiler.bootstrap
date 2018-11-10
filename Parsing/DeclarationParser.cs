using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class DeclarationParser : IDeclarationParser
    {
        [NotNull] private readonly IListParser listParser;
        [NotNull] private readonly IExpressionParser expressionParser;
        [NotNull] private readonly IBlockParser blockParser;
        [NotNull] private readonly IParameterParser parameterParser;
        [NotNull] private readonly IModifierParser modifierParser;
        [NotNull] private readonly IGenericsParser genericsParser;
        [NotNull] private readonly INameParser nameParser;
        [NotNull] private readonly IUsingDirectiveParser usingDirectiveParser;

        public DeclarationParser(
            [NotNull] IListParser listParser,
            [NotNull] IExpressionParser expressionParser,
            [NotNull] IBlockParser blockParser,
            [NotNull] IParameterParser parameterParser,
            [NotNull] IModifierParser modifierParser,
            [NotNull] IGenericsParser genericsParser,
            [NotNull] INameParser nameParser,
            [NotNull] IUsingDirectiveParser usingDirectiveParser)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
            this.blockParser = blockParser;
            this.parameterParser = parameterParser;
            this.modifierParser = modifierParser;
            this.genericsParser = genericsParser;
            this.nameParser = nameParser;
            this.usingDirectiveParser = usingDirectiveParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public SyntaxList<DeclarationSyntax> ParseDeclarations(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var declarations = new List<DeclarationSyntax>();
            while (!tokens.AtEndOfFile())
            {
                declarations.Add(ParseDeclaration(tokens, diagnostics));
            }

            return declarations.ToSyntaxList();
        }

        [MustUseReturnValue]
        [NotNull]
        public DeclarationSyntax ParseDeclaration(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var attributes = ParseAttributes(tokens, diagnostics);
            var modifiers = ParseModifiers(tokens, diagnostics);

            switch (tokens.Current)
            {
                case INamespaceKeywordToken _:
                    return ParseNamespace(attributes, modifiers, tokens, diagnostics);
                case IClassKeywordToken _:
                    return ParseClass(attributes, modifiers, tokens, diagnostics);
                case ITypeKeywordToken _:
                    return ParseType(attributes, modifiers, tokens, diagnostics);
                case IStructKeywordToken _:
                    return ParseStruct(attributes, modifiers, tokens, diagnostics);
                case IEnumKeywordToken _:
                    return ParseEnum(attributes, modifiers, tokens, diagnostics);
                case IFunctionKeywordToken _:
                    return ParseNamedFunction(attributes, modifiers, tokens, diagnostics);
                case IConstKeywordToken _:
                    return ParseConst(attributes, modifiers, tokens, diagnostics);
                default:
                    return ParseIncompleteDeclaration(attributes, modifiers, tokens, diagnostics);
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private BlockNamespaceDeclarationSyntax ParseNamespace(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var namespaceKeyword = tokens.Take<INamespaceKeywordToken>();
            var name = nameParser.ParseName(tokens, diagnostics);
            var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
            var usingDirectives = usingDirectiveParser.ParseUsingDirectives(tokens, diagnostics).ToSyntaxList();
            var declarations = ParseDeclarations(tokens, diagnostics).ToSyntaxList();
            var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
            return new BlockNamespaceDeclarationSyntax(attributes, modifiers, namespaceKeyword, name, openBrace,
                usingDirectives, declarations, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        public MemberDeclarationSyntax ParseMemberDeclaration(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var attributes = ParseAttributes(tokens, diagnostics);
            var modifiers = ParseModifiers(tokens, diagnostics);

            switch (tokens.Current)
            {
                case IClassKeywordToken _:
                    return ParseClass(attributes, modifiers, tokens, diagnostics);
                case ITypeKeywordToken _:
                    return ParseType(attributes, modifiers, tokens, diagnostics);
                case IStructKeywordToken _:
                    return ParseStruct(attributes, modifiers, tokens, diagnostics);
                case IEnumKeywordToken _:
                    return ParseEnum(attributes, modifiers, tokens, diagnostics);
                case IFunctionKeywordToken _:
                    return ParseNamedFunction(attributes, modifiers, tokens, diagnostics);
                case IOperatorKeywordToken _:
                    return ParseOperatorFunction(attributes, modifiers, tokens, diagnostics);
                case INewKeywordToken _:
                    return ParseConstructor(attributes, modifiers, tokens, diagnostics);
                case IInitKeywordToken _:
                    return ParseInitializer(attributes, modifiers, tokens, diagnostics);
                case IDeleteKeywordToken _:
                    return ParseDestructor(attributes, modifiers, tokens, diagnostics);
                case IGetKeywordToken _:
                    return ParseGetterFunction(attributes, modifiers, tokens, diagnostics);
                case ISetKeywordToken _:
                    return ParseSetterFunction(attributes, modifiers, tokens, diagnostics);
                case IVarKeywordToken _:
                case ILetKeywordToken _:
                    return ParseField(attributes, modifiers, tokens, diagnostics);
                case IConstKeywordToken _:
                    return ParseConst(attributes, modifiers, tokens, diagnostics);
                default:
                    return ParseIncompleteDeclaration(attributes, modifiers, tokens, diagnostics);
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
            }
        }

        #region Parse Type Parts
        [MustUseReturnValue]
        [NotNull]
        private SyntaxList<AttributeSyntax> ParseAttributes(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var attributes = new List<AttributeSyntax>();
            // Take modifiers until null
            while (AcceptAttribute(tokens, diagnostics) is AttributeSyntax attribute)
                attributes.Add(attribute);
            return new SyntaxList<AttributeSyntax>(attributes);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private AttributeSyntax AcceptAttribute(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var hash = tokens.Accept<IHashToken>();
            if (hash == null) return null;
            var name = nameParser.ParseName(tokens, diagnostics);
            var openParen = tokens.Accept<IOpenParenToken>();
            SeparatedListSyntax<ArgumentSyntax> argumentList = null;
            ICloseParenTokenPlace closeParen = null;
            if (openParen != null)
            {
                argumentList = expressionParser.ParseArgumentList(tokens, diagnostics);
                closeParen = tokens.Expect<ICloseParenTokenPlace>();
            }
            return new AttributeSyntax(hash, name, openParen, argumentList, closeParen);
        }

        [MustUseReturnValue]
        [NotNull]
        private SyntaxList<ModifierSyntax> ParseModifiers(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
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
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var colon = tokens.Accept<IColonToken>();
            if (colon == null) return null;
            var typeExpression = expressionParser.ParseExpression(tokens, diagnostics);
            return new BaseClassSyntax(colon, typeExpression);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private BaseTypesSyntax AcceptBaseTypes(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var lessThanColon = tokens.Accept<ILessThanColonToken>();
            if (lessThanColon == null) return null;
            var typeExpressions = listParser.ParseSeparatedList(tokens, expressionParser.ParseExpression,
                TypeOf<ICommaToken>(), TypeOf<IOpenBraceToken>(), diagnostics);
            return new BaseTypesSyntax(lessThanColon, typeExpressions);
        }

        [MustUseReturnValue]
        [NotNull]
        private SyntaxList<InvariantSyntax> ParseInvariants(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            return listParser.ParseList(tokens, AcceptInvariant, diagnostics);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private InvariantSyntax AcceptInvariant(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var invariantKeyword = tokens.Accept<IInvariantKeywordToken>();
            if (invariantKeyword == null) return null;
            var condition = expressionParser.ParseExpression(tokens, diagnostics);
            return new InvariantSyntax(invariantKeyword, condition);
        }
        #endregion

        #region Parse Type Declarations
        [MustUseReturnValue]
        [NotNull]
        private ClassDeclarationSyntax ParseClass(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var classKeyword = tokens.Take<IClassKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
            var baseClass = AcceptBaseClass(tokens, diagnostics);
            var baseTypes = AcceptBaseTypes(tokens, diagnostics);
            var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
            var invariants = ParseInvariants(tokens, diagnostics);
            var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
            var members = listParser.ParseList(tokens, ParseMemberDeclaration, TypeOf<ICloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
            return new ClassDeclarationSyntax(modifiers, classKeyword, name, genericParameters,
                baseClass, baseTypes, genericConstraints, invariants, openBrace,
                members, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        private TypeDeclarationSyntax ParseType(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var typeKeyword = tokens.Take<ITypeKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
            var baseTypes = AcceptBaseTypes(tokens, diagnostics);
            var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
            var invariants = ParseInvariants(tokens, diagnostics);
            var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
            var members = listParser.ParseList(tokens, ParseMemberDeclaration, TypeOf<ICloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
            return new TypeDeclarationSyntax(attributes, modifiers, typeKeyword, name,
                genericParameters, baseTypes, genericConstraints, invariants, openBrace,
                members, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        private StructDeclarationSyntax ParseStruct(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var structKeyword = tokens.Take<IStructKeywordToken>();
            var name = tokens.Expect<IIdentifierOrPrimitiveTokenPlace>();
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
            var baseTypes = AcceptBaseTypes(tokens, diagnostics);
            var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
            var invariants = ParseInvariants(tokens, diagnostics);
            var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
            var members = listParser.ParseList(tokens, ParseMemberDeclaration, TypeOf<ICloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
            return new StructDeclarationSyntax(attributes, modifiers, structKeyword, name, genericParameters,
                baseTypes, genericConstraints, invariants, openBrace,
                members, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        private MemberDeclarationSyntax ParseEnum(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var enumKeyword = tokens.Take<IEnumKeywordToken>();
            switch (tokens.Current)
            {
                case IStructKeywordToken _:
                {
                    var structKeyword = tokens.Take<IStructKeywordToken>();
                    var name = tokens.ExpectIdentifier();
                    var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
                    var baseTypes = AcceptBaseTypes(tokens, diagnostics);
                    var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
                    var invariants = ParseInvariants(tokens, diagnostics);
                    var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
                    var variants = ParseEnumVariants(tokens, diagnostics);
                    var members = listParser.ParseList(tokens, ParseMemberDeclaration,
                        TypeOf<ICloseBraceToken>(), diagnostics);
                    var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
                    return new EnumStructDeclarationSyntax(modifiers, enumKeyword, structKeyword,
                        name, genericParameters, baseTypes, genericConstraints,
                        invariants, openBrace, variants, members, closeBrace);
                }
                case IClassKeywordToken _:
                {
                    var classKeyword = tokens.Expect<IClassKeywordToken>();
                    var name = tokens.ExpectIdentifier();
                    var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
                    var baseClass = AcceptBaseClass(tokens, diagnostics);
                    var baseTypes = AcceptBaseTypes(tokens, diagnostics);
                    var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
                    var invariants = ParseInvariants(tokens, diagnostics);
                    var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
                    var variants = ParseEnumVariants(tokens, diagnostics);
                    var members = listParser.ParseList(tokens, ParseMemberDeclaration,
                        TypeOf<ICloseBraceToken>(), diagnostics);
                    var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
                    return new EnumClassDeclarationSyntax(modifiers, enumKeyword, classKeyword,
                        name, genericParameters, baseClass, baseTypes, genericConstraints,
                        invariants, openBrace, variants, members, closeBrace);
                }
                default:
                    return ParseIncompleteDeclaration(attributes, modifiers, tokens, diagnostics);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private EnumVariantsSyntax ParseEnumVariants(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var variants = new List<EnumVariantSyntax>();
            while (tokens.Current is IIdentifierToken)
                variants.Add(ParseEnumVariant(tokens, diagnostics));

            ISemicolonTokenPlace semicolon = null;
            if (!(tokens.Current is ICloseBraceToken))
                semicolon = tokens.Expect<ISemicolonTokenPlace>();

            return new EnumVariantsSyntax(variants.ToSyntaxList(), semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        private EnumVariantSyntax ParseEnumVariant(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var identifier = tokens.ExpectIdentifier();
            var comma = tokens.Accept<ICommaToken>();
            return new EnumVariantSyntax(identifier, comma);
        }
        #endregion

        #region Parse Type Member Declarations
        [MustUseReturnValue]
        [NotNull]
        private FieldDeclarationSyntax ParseField(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var binding = tokens.Take<IBindingToken>();
            var getter = AcceptFieldGetter(tokens, diagnostics);
            var name = tokens.ExpectIdentifier();
            IColonTokenPlace colon = null;
            ExpressionSyntax typeExpression = null;
            if (tokens.Current is IColonToken)
            {
                colon = tokens.Expect<IColonTokenPlace>();
                // Need to not consume the assignment that separates the type from the initializer,
                // hence the min operator precedence.
                typeExpression = expressionParser.ParseExpression(tokens, diagnostics, OperatorPrecedence.AboveAssignment);
            }
            IEqualsToken equals = null;
            ExpressionSyntax initializer = null;
            if (tokens.Current is IEqualsToken)
            {
                equals = tokens.Take<IEqualsToken>();
                initializer = expressionParser.ParseExpression(tokens, diagnostics);
            }
            var semicolon = tokens.Expect<ISemicolonTokenPlace>();
            return new FieldDeclarationSyntax(modifiers, binding, getter, name, colon, typeExpression,
                equals, initializer, semicolon);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private static FieldGetterSyntax AcceptFieldGetter(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var publicKeyword = tokens.Accept<IPublicKeywordToken>();
            var getKeyword = tokens.Accept<IGetKeywordToken>();
            if (publicKeyword == null && getKeyword == null) return null;
            return new FieldGetterSyntax(publicKeyword, getKeyword);
        }
        #endregion

        #region Parse Functions
        private (BlockSyntax, ISemicolonTokenPlace) ParseFunctionBody(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var body = blockParser.AcceptBlock(tokens, diagnostics);
            ISemicolonTokenPlace semicolon = null;
            if (body == null)
                semicolon = tokens.Expect<ISemicolonTokenPlace>();
            return (body, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        public NamedFunctionDeclarationSyntax ParseNamedFunction(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var functionKeyword = tokens.Take<IFunctionKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);

            IOpenParenTokenPlace openParen;
            SeparatedListSyntax<ParameterSyntax> parameters;
            ICloseParenTokenPlace closeParen;
            if (tokens.Current is IOpenParenToken)
            {
                openParen = tokens.Take<IOpenParenToken>();
                parameters = ParseParameterList(tokens, diagnostics);
                closeParen = tokens.Expect<ICloseParenTokenPlace>();
            }
            else
            {
                // TODO handle this better
                openParen = tokens.Missing<IOpenParenTokenPlace>();
                parameters = SeparatedListSyntax<ParameterSyntax>.Empty;
                closeParen = tokens.Missing<ICloseParenTokenPlace>();
            }
            var arrow = tokens.Expect<IRightArrowTokenPlace>();
            var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
            var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseFunctionContracts(tokens, diagnostics);
            var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
            return new NamedFunctionDeclarationSyntax(modifiers, functionKeyword, name, genericParameters,
                openParen, parameters, closeParen, arrow, returnTypeExpression,
                genericConstraints, effects, contracts, body, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        public OperatorFunctionDeclarationSyntax ParseOperatorFunction(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var operatorKeyword = tokens.Take<IOperatorKeywordToken>();
            // TODO save the generic parameters
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
            IOperatorTokenPlace @operator;
            // TODO correctly store these in the syntax class
            switch (tokens.Current)
            {
                case IHashToken _:
                    @operator = tokens.Expect<IOperatorTokenPlace>();
                    tokens.Next();
                    switch (tokens.Current)
                    {
                        case IOpenParenToken _:
                            tokens.Next();
                            tokens.Expect<ICloseParenTokenPlace>();
                            break;
                        case IOpenBracketToken _:
                            tokens.Next();
                            tokens.Expect<ICloseBracketTokenPlace>();
                            break;
                        case IOpenBraceToken _:
                            tokens.Next();
                            tokens.Expect<ICloseBraceTokenPlace>();
                            break;
                        default:
                            tokens.Expect<IOpenBracketTokenPlace>();
                            break;
                    }
                    break;
                case IStringLiteralToken _:
                    @operator = tokens.Expect<IOperatorTokenPlace>();
                    tokens.Next();
                    // TODO need to check it is empty string
                    break;
                // TODO case for user defined literals ''
                default:
                    @operator = tokens.Expect<IOperatorTokenPlace>();
                    break;
            }
            var openParen = tokens.Expect<IOpenParenTokenPlace>();
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenTokenPlace>();
            var arrow = tokens.Expect<IRightArrowTokenPlace>();
            var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
            var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseFunctionContracts(tokens, diagnostics);
            var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
            return new OperatorFunctionDeclarationSyntax(modifiers, operatorKeyword, @operator,
                openParen, parameters, closeParen, arrow, returnTypeExpression,
                genericConstraints, effects, contracts, body, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        public ConstructorFunctionDeclarationSyntax ParseConstructor(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var newKeyword = tokens.Take<INewKeywordToken>();
            var name = tokens.Accept<IIdentifierToken>();
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
            var openParen = tokens.Expect<IOpenParenTokenPlace>();
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenTokenPlace>();
            var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseFunctionContracts(tokens, diagnostics);
            var body = blockParser.ParseBlock(tokens, diagnostics);
            return new ConstructorFunctionDeclarationSyntax(modifiers, newKeyword, name,
                genericParameters, openParen, parameters, closeParen, genericConstraints, effects, contracts, body);
        }

        [MustUseReturnValue]
        [NotNull]
        public InitializerFunctionDeclarationSyntax ParseInitializer(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var initKeyword = tokens.Take<IInitKeywordToken>();
            var name = tokens.Accept<IIdentifierToken>();
            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
            var openParen = tokens.Expect<IOpenParenTokenPlace>();
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenTokenPlace>();
            var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseFunctionContracts(tokens, diagnostics);
            var body = blockParser.ParseBlock(tokens, diagnostics);
            return new InitializerFunctionDeclarationSyntax(modifiers, initKeyword, name,
                genericParameters, openParen, parameters, closeParen, genericConstraints, effects, contracts, body);
        }


        [MustUseReturnValue]
        [NotNull]
        public DestructorFunctionDeclarationSyntax ParseDestructor(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var deleteKeyword = tokens.Take<IDeleteKeywordToken>();
            var openParen = tokens.Expect<IOpenParenTokenPlace>();
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenTokenPlace>();
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseFunctionContracts(tokens, diagnostics);
            var body = blockParser.ParseBlock(tokens, diagnostics);
            return new DestructorFunctionDeclarationSyntax(modifiers, deleteKeyword,
                 openParen, parameters, closeParen, effects, contracts, body);
        }

        [MustUseReturnValue]
        [NotNull]
        private GetterFunctionDeclarationSyntax ParseGetterFunction(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var getKeyword = tokens.Take<IGetKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var openParen = tokens.Expect<IOpenParenTokenPlace>();
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenTokenPlace>();
            var arrow = tokens.Expect<IRightArrowTokenPlace>();
            var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseFunctionContracts(tokens, diagnostics);
            var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
            return new GetterFunctionDeclarationSyntax(modifiers, getKeyword, name,
                openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        private SetterFunctionDeclarationSyntax ParseSetterFunction(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var setKeyword = tokens.Take<ISetKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var openParen = tokens.Expect<IOpenParenTokenPlace>();
            var parameters = ParseParameterList(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenTokenPlace>();
            var arrow = tokens.Expect<IRightArrowTokenPlace>();
            var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
            var effects = AcceptEffects(tokens, diagnostics);
            var contracts = ParseFunctionContracts(tokens, diagnostics);
            var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
            return new SetterFunctionDeclarationSyntax(modifiers, setKeyword, name,
                openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        private SeparatedListSyntax<ParameterSyntax> ParseParameterList(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            return listParser.ParseSeparatedList(tokens, parameterParser.ParseParameter, TypeOf<ICommaToken>(), TypeOf<ICloseParenToken>(), diagnostics);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private EffectsSyntax AcceptEffects(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            if (!(tokens.Current is INoKeywordToken) && !(tokens.Current is IMayKeywordToken))
                return null;

            var mayKeyword = tokens.Expect<IMayKeywordTokenPlace>();
            var allowedEffects = mayKeyword is IMissingToken ? SeparatedListSyntax<EffectSyntax>.Empty
                : listParser.ParseSeparatedList(tokens, ParseEffect,
                TypeOf<ICommaToken>(), TypeOf<IOpenBraceToken>(), diagnostics);
            var noKeyword = tokens.Expect<INoKeywordTokenPlace>();
            var disallowedEffects = noKeyword is IMissingToken ? SeparatedListSyntax<EffectSyntax>.Empty
                : listParser.ParseSeparatedList(tokens, ParseEffect,
                TypeOf<ICommaToken>(), TypeOf<IOpenBraceToken>(), diagnostics);
            return new EffectsSyntax(mayKeyword, allowedEffects, noKeyword, disallowedEffects);
        }

        [MustUseReturnValue]
        [NotNull]
        public EffectSyntax ParseEffect(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            switch (tokens.Current)
            {
                case IThrowKeywordToken throwKeyword:
                    tokens.Next();
                    var exceptions = listParser.ParseSeparatedList(tokens, ParseThrowEffectEntry, TypeOf<ICommaToken>(), TypeOf<IOpenBraceToken>(), diagnostics);
                    return new ThrowEffectSyntax(throwKeyword, exceptions);
                case IIdentifierToken identifier:
                    tokens.Next();
                    return new SimpleEffectSyntax(identifier);
                default:
                    throw NonExhaustiveMatchException.For(tokens.Current);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private ThrowEffectEntrySyntax ParseThrowEffectEntry(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var paramsKeyword = tokens.Accept<IParamsKeywordToken>();
            var exceptionType = expressionParser.ParseExpression(tokens, diagnostics);
            return new ThrowEffectEntrySyntax(paramsKeyword, exceptionType);
        }

        [MustUseReturnValue]
        [NotNull]
        private SyntaxList<FunctionContractSyntax> ParseFunctionContracts(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var contracts = new List<FunctionContractSyntax>();
            for (; ; )
                switch (tokens.Current)
                {
                    case IRequiresKeywordToken _:
                        contracts.Add(ParseRequires(tokens, diagnostics));
                        break;
                    case IEnsuresKeywordToken _:
                        contracts.Add(ParseEnsures(tokens, diagnostics));
                        break;
                    default:
                        return contracts.ToSyntaxList();
                }
        }

        [MustUseReturnValue]
        [NotNull]
        private RequiresSyntax ParseRequires(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var requiresKeyword = tokens.Take<IRequiresKeywordToken>();
            var condition = expressionParser.ParseExpression(tokens, diagnostics);
            return new RequiresSyntax(requiresKeyword, condition);
        }

        [MustUseReturnValue]
        [NotNull]
        private EnsuresSyntax ParseEnsures(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var ensuresKeyword = tokens.Take<IEnsuresKeywordToken>();
            var condition = expressionParser.ParseExpression(tokens, diagnostics);
            return new EnsuresSyntax(ensuresKeyword, condition);
        }
        #endregion

        [MustUseReturnValue]
        [NotNull]
        private ConstDeclarationSyntax ParseConst(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var constKeyword = tokens.Take<IConstKeywordToken>();
            var name = tokens.ExpectIdentifier();
            IColonTokenPlace colon = null;
            ExpressionSyntax typeExpression = null;
            if (tokens.Current is IColonToken)
            {
                colon = tokens.Expect<IColonTokenPlace>();
                // Need to not consume the assignment that separates the type from the initializer,
                // hence the min operator precedence.
                typeExpression = expressionParser.ParseExpression(tokens, diagnostics, OperatorPrecedence.AboveAssignment);
            }
            IEqualsToken equals = null;
            ExpressionSyntax initializer = null;
            if (tokens.Current is IEqualsToken)
            {
                equals = tokens.Take<IEqualsToken>();
                initializer = expressionParser.ParseExpression(tokens, diagnostics);
            }
            var semicolon = tokens.Expect<ISemicolonTokenPlace>();
            return new ConstDeclarationSyntax(attributes, modifiers, constKeyword, name, colon, typeExpression,
                equals, initializer, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        private static IncompleteDeclarationSyntax ParseIncompleteDeclaration(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] IEnumerable<ModifierSyntax> modifiers,
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var skipped = attributes.SelectMany(a => a.Tokens()).Concat(modifiers.SelectMany(m => m.Tokens())).ToList();
            skipped.Add(tokens.ExpectIdentifier());    // The name we are expecting

            if (skipped.All(s => s is IMissingToken))
            {
                // We haven't consumed any tokens, we need to consume a token so
                // we make progress.
                skipped.Add(tokens.Current);
                tokens.Next();
            }

            var span = TextSpan.Covering(
                skipped.First(s => s != null).NotNull().Span,
                skipped.Last(s => s != null).NotNull().Span);

            diagnostics.Add(ParseError.IncompleteDeclaration(tokens.Context.File, span));
            return new IncompleteDeclarationSyntax(skipped);
        }
    }
}
