using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class DeclarationParser : Parser, IDeclarationParser
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
            [NotNull] ITokenIterator tokens,
            [NotNull] IListParser listParser,
            [NotNull] IExpressionParser expressionParser,
            [NotNull] IBlockParser blockParser,
            [NotNull] IParameterParser parameterParser,
            [NotNull] IModifierParser modifierParser,
            [NotNull] IGenericsParser genericsParser,
            [NotNull] INameParser nameParser,
            [NotNull] IUsingDirectiveParser usingDirectiveParser)
            : base(tokens)
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
        [NotNull, ItemNotNull]
        public FixedList<DeclarationSyntax> ParseDeclarations()
        {
            var declarations = new List<DeclarationSyntax>();
            while (!Tokens.AtEndOfFile())
                declarations.Add(ParseDeclaration());

            return declarations.ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        public DeclarationSyntax ParseDeclaration()
        {
            var attributes = ParseAttributes();
            var modifiers = listParser.ParseList(Tokens.Accept<IModiferToken>);

            switch (Tokens.Current)
            {
                case INamespaceKeywordToken _:
                    return ParseNamespaceDeclaration(attributes, modifiers);
                //case IClassKeywordToken _:
                //    return ParseClass(attributes, modifiers);
                //case ITypeKeywordToken _:
                //    return ParseType(attributes, modifiers);
                //case IStructKeywordToken _:
                //    return ParseStruct(attributes, modifiers);
                //case IEnumKeywordToken _:
                //    return ParseEnum(attributes, modifiers);
                case IFunctionKeywordToken _:
                    return ParseNamedFunction(attributes, modifiers);
                //case IConstKeywordToken _:
                //    return ParseConst(attributes, modifiers);
                //default:
                //    return ParseIncompleteDeclaration(attributes, modifiers);
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
                default:
                    throw NonExhaustiveMatchException.For(Tokens.Current);
            }
        }

        //[MustUseReturnValue]
        //[NotNull]
        //public MemberDeclarationSyntax ParseMemberDeclaration()
        //{
        //    var attributes = ParseAttributes(Tokens, Tokens.Context.Diagnostics);
        //    var modifiers = listParser.ParseList(Tokens.Accept<IModiferToken>);

        //    switch (Tokens.Current)
        //    {
        //        case IClassKeywordToken _:
        //            return ParseClass(attributes, modifiers);
        //        case ITypeKeywordToken _:
        //            return ParseType(attributes, modifiers);
        //        case IStructKeywordToken _:
        //            return ParseStruct(attributes, modifiers);
        //        case IEnumKeywordToken _:
        //            return ParseEnum(attributes, modifiers);
        //        case IFunctionKeywordToken _:
        //            return ParseNamedFunction(attributes, modifiers);
        //        case IOperatorKeywordToken _:
        //            return ParseOperatorFunction(attributes, modifiers);
        //        case INewKeywordToken _:
        //            return ParseConstructor(attributes, modifiers);
        //        case IInitKeywordToken _:
        //            return ParseInitializer(attributes, modifiers);
        //        case IDeleteKeywordToken _:
        //            return ParseDestructor(attributes, modifiers);
        //        case IGetKeywordToken _:
        //            return ParseGetterFunction(attributes, modifiers);
        //        case ISetKeywordToken _:
        //            return ParseSetterFunction(attributes, modifiers);
        //        case IVarKeywordToken _:
        //        case ILetKeywordToken _:
        //            return ParseField(attributes, modifiers);
        //        case IConstKeywordToken _:
        //            return ParseConst(attributes, modifiers);
        //        default:
        //            return ParseIncompleteDeclaration(attributes, modifiers);
        //        case null:
        //            throw new InvalidOperationException("Can't parse past end of file");
        //    }
        //}

        #region Parse Namespaces
        [MustUseReturnValue]
        [NotNull]
        public NamespaceDeclarationSyntax ParseFileNamespace([NotNull] FixedList<string> name)
        {
            var usingDirectives = usingDirectiveParser.ParseUsingDirectives(Tokens, Tokens.Context.Diagnostics).ToFixedList();
            var declarations = ParseDeclarations().ToFixedList();
            return new NamespaceDeclarationSyntax(true, name, null, usingDirectives, declarations);
        }

        [MustUseReturnValue]
        [NotNull]
        public NamespaceDeclarationSyntax ParseNamespaceDeclaration(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            // TODO generate errors for attributes or modifiers
            Tokens.Required<INamespaceKeywordToken>();
            var globalQualifier = Tokens.Accept<IColonColonToken>();
            var (name, span) = ParseNamespaceName();
            span = TextSpan.Covering(span, globalQualifier?.Span);
            Tokens.Expect<IOpenBraceToken>();
            var usingDirectives = usingDirectiveParser.ParseUsingDirectives(Tokens, Tokens.Context.Diagnostics).ToFixedList();
            var declarations = ParseDeclarations();
            Tokens.Expect<ICloseBraceToken>();
            return new NamespaceDeclarationSyntax(globalQualifier != null, name, span, usingDirectives, declarations);
        }

        [MustUseReturnValue]
        private (FixedList<string>, TextSpan) ParseNamespaceName()
        {
            var name = new List<string>();

            var nameSegment = Tokens.RequiredIdentifier();
            if (nameSegment == null)
                return (FixedList<string>.Empty, Tokens.Current.NotNull().Span);
            var span = nameSegment.Span;
            name.Add(nameSegment.Value);

            while (Tokens.Accept<IDotToken>() != null)
            {
                nameSegment = Tokens.ExpectIdentifier();
                if (nameSegment == null) break;
                span = TextSpan.Covering(span, nameSegment.Span);
                name.Add(nameSegment.Value);
            }

            return (name.ToFixedList(), span);
        }
        #endregion

        #region Parse Type Parts
        [MustUseReturnValue]
        [NotNull]
        private FixedList<AttributeSyntax> ParseAttributes()
        {
            var attributes = new List<AttributeSyntax>();
            // Take modifiers until null
            while (AcceptAttribute() is AttributeSyntax attribute)
                attributes.Add(attribute);
            return attributes.ToFixedList();
        }

        [MustUseReturnValue]
        [CanBeNull]
        private AttributeSyntax AcceptAttribute()
        {
            var hash = Tokens.Accept<IHashToken>();
            if (hash == null) return null;
            var name = nameParser.ParseName(Tokens, Tokens.Context.Diagnostics);
            var openParen = Tokens.Accept<IOpenParenToken>();
            SeparatedListSyntax<ArgumentSyntax> argumentList = null;
            ICloseParenTokenPlace closeParen = null;
            if (openParen != null)
            {
                argumentList = expressionParser.ParseArgumentList(Tokens, Tokens.Context.Diagnostics);
                closeParen = Tokens.Consume<ICloseParenTokenPlace>();
            }
            return new AttributeSyntax(hash, name, openParen, argumentList, closeParen);
        }

        //[MustUseReturnValue]
        //[NotNull]
        //private SyntaxList<ModifierSyntax> ParseModifiers(
        //    [NotNull] ITokenIterator tokens,
        //    [NotNull] Diagnostics diagnostics)
        //{
        //    var modifiers = new List<ModifierSyntax>();
        //    // Take modifiers until null
        //    while (modifierParser.AcceptModifier(tokens, diagnostics) is ModifierSyntax modifier)
        //        modifiers.Add(modifier);
        //    return new SyntaxList<ModifierSyntax>(modifiers);
        //}

        //[MustUseReturnValue]
        //[CanBeNull]
        //private BaseClassSyntax AcceptBaseClass(
        //    [NotNull] ITokenIterator tokens,
        //    [NotNull] Diagnostics diagnostics)
        //{
        //    var colon = tokens.Accept<IColonToken>();
        //    if (colon == null) return null;
        //    var typeExpression = expressionParser.ParseExpression(tokens, diagnostics);
        //    return new BaseClassSyntax(colon, typeExpression);
        //}

        //[MustUseReturnValue]
        //[CanBeNull]
        //private BaseTypesSyntax AcceptBaseTypes(
        //    [NotNull] ITokenIterator tokens,
        //    [NotNull] Diagnostics diagnostics)
        //{
        //    var lessThanColon = tokens.Accept<ILessThanColonToken>();
        //    if (lessThanColon == null) return null;
        //    var typeExpressions = listParser.ParseSeparatedList(tokens, expressionParser.ParseExpression,
        //        TypeOf<ICommaToken>(), TypeOf<IOpenBraceToken>(), diagnostics);
        //    return new BaseTypesSyntax(lessThanColon, typeExpressions);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //private SyntaxList<InvariantSyntax> ParseInvariants(
        //    [NotNull] ITokenIterator tokens,
        //    [NotNull] Diagnostics diagnostics)
        //{
        //    return listParser.ParseList(tokens, AcceptInvariant, diagnostics);
        //}

        //[MustUseReturnValue]
        //[CanBeNull]
        //private InvariantSyntax AcceptInvariant(
        //    [NotNull] ITokenIterator tokens,
        //    [NotNull] Diagnostics diagnostics)
        //{
        //    var invariantKeyword = tokens.Accept<IInvariantKeywordToken>();
        //    if (invariantKeyword == null) return null;
        //    var condition = expressionParser.ParseExpression(tokens, diagnostics);
        //    return new InvariantSyntax(invariantKeyword, condition);
        //}
        #endregion

        #region Parse Type Declarations
        //[MustUseReturnValue]
        //[NotNull]
        //private ClassDeclarationSyntax ParseClass(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var classKeyword = tokens.Take<IClassKeywordToken>();
        //    var name = tokens.ExpectIdentifier();
        //    var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
        //    var baseClass = AcceptBaseClass(tokens, diagnostics);
        //    var baseTypes = AcceptBaseTypes(tokens, diagnostics);
        //    var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
        //    var invariants = ParseInvariants(tokens, diagnostics);
        //    var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
        //    var members = listParser.ParseList(tokens, (tokens, diagnostics) => ParseMemberDeclaration(), TypeOf<ICloseBraceToken>(), diagnostics);
        //    var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
        //    return new ClassDeclarationSyntax(modifiers, classKeyword, name, genericParameters,
        //        baseClass, baseTypes, genericConstraints, invariants, openBrace,
        //        members, closeBrace);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //private TypeDeclarationSyntax ParseType(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var typeKeyword = tokens.Take<ITypeKeywordToken>();
        //    var name = tokens.ExpectIdentifier();
        //    var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
        //    var baseTypes = AcceptBaseTypes(tokens, diagnostics);
        //    var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
        //    var invariants = ParseInvariants(tokens, diagnostics);
        //    var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
        //    var members = listParser.ParseList(tokens, (tokens, diagnostics) => ParseMemberDeclaration(), TypeOf<ICloseBraceToken>(), diagnostics);
        //    var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
        //    return new TypeDeclarationSyntax(attributes, modifiers, typeKeyword, name,
        //        genericParameters, baseTypes, genericConstraints, invariants, openBrace,
        //        members, closeBrace);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //private StructDeclarationSyntax ParseStruct(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var structKeyword = tokens.Take<IStructKeywordToken>();
        //    var name = tokens.Expect<IIdentifierOrPrimitiveTokenPlace>();
        //    var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
        //    var baseTypes = AcceptBaseTypes(tokens, diagnostics);
        //    var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
        //    var invariants = ParseInvariants(tokens, diagnostics);
        //    var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
        //    var members = listParser.ParseList(tokens, (tokens, diagnostics) => ParseMemberDeclaration(), TypeOf<ICloseBraceToken>(), diagnostics);
        //    var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
        //    return new StructDeclarationSyntax(attributes, modifiers, structKeyword, name, genericParameters,
        //        baseTypes, genericConstraints, invariants, openBrace,
        //        members, closeBrace);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //private MemberDeclarationSyntax ParseEnum(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var enumKeyword = tokens.Take<IEnumKeywordToken>();
        //    switch (tokens.Current)
        //    {
        //        case IStructKeywordToken _:
        //        {
        //            var structKeyword = tokens.Take<IStructKeywordToken>();
        //            var name = tokens.ExpectIdentifier();
        //            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
        //            var baseTypes = AcceptBaseTypes(tokens, diagnostics);
        //            var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
        //            var invariants = ParseInvariants(tokens, diagnostics);
        //            var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
        //            var variants = ParseEnumVariants(tokens, diagnostics);
        //            var members = listParser.ParseList(tokens, (tokens, diagnostics) => ParseMemberDeclaration(),
        //                TypeOf<ICloseBraceToken>(), diagnostics);
        //            var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
        //            return new EnumStructDeclarationSyntax(modifiers, enumKeyword, structKeyword,
        //                name, genericParameters, baseTypes, genericConstraints,
        //                invariants, openBrace, variants, members, closeBrace);
        //        }
        //        case IClassKeywordToken _:
        //        {
        //            var classKeyword = tokens.Expect<IClassKeywordToken>();
        //            var name = tokens.ExpectIdentifier();
        //            var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
        //            var baseClass = AcceptBaseClass(tokens, diagnostics);
        //            var baseTypes = AcceptBaseTypes(tokens, diagnostics);
        //            var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
        //            var invariants = ParseInvariants(tokens, diagnostics);
        //            var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
        //            var variants = ParseEnumVariants(tokens, diagnostics);
        //            var members = listParser.ParseList(tokens, (tokens, diagnostics) => ParseMemberDeclaration(),
        //                TypeOf<ICloseBraceToken>(), diagnostics);
        //            var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
        //            return new EnumClassDeclarationSyntax(modifiers, enumKeyword, classKeyword,
        //                name, genericParameters, baseClass, baseTypes, genericConstraints,
        //                invariants, openBrace, variants, members, closeBrace);
        //        }
        //        default:
        //            return ParseIncompleteDeclaration(attributes, modifiers);
        //    }
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //private EnumVariantsSyntax ParseEnumVariants(
        //    [NotNull] ITokenIterator tokens,
        //    [NotNull] Diagnostics diagnostics)
        //{
        //    var variants = new List<EnumVariantSyntax>();
        //    while (tokens.Current is IIdentifierToken)
        //        variants.Add(ParseEnumVariant(tokens, diagnostics));

        //    ISemicolonTokenPlace semicolon = null;
        //    if (!(tokens.Current is ICloseBraceToken))
        //        semicolon = tokens.Consume<ISemicolonTokenPlace>();

        //    return new EnumVariantsSyntax(variants.ToSyntaxList(), semicolon);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //private EnumVariantSyntax ParseEnumVariant(
        //    [NotNull] ITokenIterator tokens,
        //    [NotNull] Diagnostics diagnostics)
        //{
        //    var identifier = tokens.ExpectIdentifier();
        //    var comma = tokens.Accept<ICommaToken>();
        //    return new EnumVariantSyntax(identifier, comma);
        //}
        #endregion

        #region Parse Type Member Declarations
        //[MustUseReturnValue]
        //[NotNull]
        //private FieldDeclarationSyntax ParseField(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var binding = tokens.Take<IBindingToken>();
        //    var getter = AcceptFieldGetter(tokens, diagnostics);
        //    var name = tokens.ExpectIdentifier();
        //    IColonTokenPlace colon = null;
        //    ExpressionSyntax typeExpression = null;
        //    if (tokens.Current is IColonToken)
        //    {
        //        colon = tokens.Expect<IColonTokenPlace>();
        //        // Need to not consume the assignment that separates the type from the initializer,
        //        // hence the min operator precedence.
        //        typeExpression = expressionParser.ParseExpression(tokens, diagnostics, OperatorPrecedence.AboveAssignment);
        //    }
        //    IEqualsToken equals = null;
        //    ExpressionSyntax initializer = null;
        //    if (tokens.Current is IEqualsToken)
        //    {
        //        equals = tokens.Take<IEqualsToken>();
        //        initializer = expressionParser.ParseExpression(tokens, diagnostics);
        //    }
        //    var semicolon = tokens.Expect<ISemicolonTokenPlace>();
        //    return new FieldDeclarationSyntax(modifiers, binding, getter, name, colon, typeExpression,
        //        equals, initializer, semicolon);
        //}

        //[MustUseReturnValue]
        //[CanBeNull]
        //private static FieldGetterSyntax AcceptFieldGetter(
        //    [NotNull] ITokenIterator tokens,
        //    [NotNull] Diagnostics diagnostics)
        //{
        //    var publicKeyword = tokens.Accept<IPublicKeywordToken>();
        //    var getKeyword = tokens.Accept<IGetKeywordToken>();
        //    if (publicKeyword == null && getKeyword == null) return null;
        //    return new FieldGetterSyntax(publicKeyword, getKeyword);
        //}
        #endregion

        #region Parse Functions
        [MustUseReturnValue]
        [NotNull]
        public NamedFunctionDeclarationSyntax ParseNamedFunction(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            Tokens.Required<IFunctionKeywordToken>();
            var name = Tokens.RequiredIdentifier();
            var genericParameters = genericsParser.AcceptGenericParameters(Tokens, Tokens.Context.Diagnostics);
            var parameters = AcceptParameters();
            Tokens.Expect<IRightArrowToken>();
            var returnTypeExpression = expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
            var genericConstraints = genericsParser.ParseGenericConstraints(Tokens, Tokens.Context.Diagnostics);
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = ParseFunctionBody(Tokens, Tokens.Context.Diagnostics);
            return new NamedFunctionDeclarationSyntax(modifiers, name, genericParameters, parameters, returnTypeExpression,
                genericConstraints, mayEffects, noEffects, requires, ensures, body);
        }

        //[MustUseReturnValue]
        //[NotNull]
        //public OperatorFunctionDeclarationSyntax ParseOperatorFunction(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var operatorKeyword = tokens.Take<IOperatorKeywordToken>();
        //    // TODO save the generic parameters
        //    var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
        //    IOperatorTokenPlace @operator;
        //    // TODO correctly store these in the syntax class
        //    switch (tokens.Current)
        //    {
        //        case IHashToken _:
        //            @operator = tokens.Expect<IOperatorTokenPlace>();
        //            tokens.Next();
        //            switch (tokens.Current)
        //            {
        //                case IOpenParenToken _:
        //                    tokens.Next();
        //                    tokens.Expect<ICloseParenTokenPlace>();
        //                    break;
        //                case IOpenBracketToken _:
        //                    tokens.Next();
        //                    tokens.Expect<ICloseBracketTokenPlace>();
        //                    break;
        //                case IOpenBraceToken _:
        //                    tokens.Next();
        //                    tokens.Expect<ICloseBraceTokenPlace>();
        //                    break;
        //                default:
        //                    tokens.Expect<IOpenBracketTokenPlace>();
        //                    break;
        //            }
        //            break;
        //        case IStringLiteralToken _:
        //            @operator = tokens.Expect<IOperatorTokenPlace>();
        //            tokens.Next();
        //            // TODO need to check it is empty string
        //            break;
        //        // TODO case for user defined literals ''
        //        default:
        //            @operator = tokens.Expect<IOperatorTokenPlace>();
        //            break;
        //    }
        //    var openParen = tokens.Expect<IOpenParenTokenPlace>();
        //    var parameters = ParseParameterList(tokens, diagnostics);
        //    var closeParen = tokens.Expect<ICloseParenTokenPlace>();
        //    var arrow = tokens.Expect<IRightArrowTokenPlace>();
        //    var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
        //    var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
        //    var effects = AcceptEffects(tokens, diagnostics);
        //    var contracts = ParseFunctionContracts(tokens, diagnostics);
        //    var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
        //    return new OperatorFunctionDeclarationSyntax(modifiers, operatorKeyword, @operator,
        //        openParen, parameters, closeParen, arrow, returnTypeExpression,
        //        genericConstraints, effects, contracts, body, semicolon);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //public ConstructorFunctionDeclarationSyntax ParseConstructor(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var newKeyword = tokens.Take<INewKeywordToken>();
        //    var name = tokens.Accept<IIdentifierToken>();
        //    var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
        //    var openParen = tokens.Expect<IOpenParenTokenPlace>();
        //    var parameters = ParseParameterList(tokens, diagnostics);
        //    var closeParen = tokens.Expect<ICloseParenTokenPlace>();
        //    var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
        //    var effects = AcceptEffects(tokens, diagnostics);
        //    var contracts = ParseFunctionContracts(tokens, diagnostics);
        //    var body = blockParser.ParseBlock(tokens, diagnostics);
        //    return new ConstructorFunctionDeclarationSyntax(modifiers, newKeyword, name,
        //        genericParameters, openParen, parameters, closeParen, genericConstraints, effects, contracts, body);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //public InitializerFunctionDeclarationSyntax ParseInitializer(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var initKeyword = tokens.Take<IInitKeywordToken>();
        //    var name = tokens.Accept<IIdentifierToken>();
        //    var genericParameters = genericsParser.AcceptGenericParameters(tokens, diagnostics);
        //    var openParen = tokens.Expect<IOpenParenTokenPlace>();
        //    var parameters = ParseParameterList(tokens, diagnostics);
        //    var closeParen = tokens.Expect<ICloseParenTokenPlace>();
        //    var genericConstraints = genericsParser.ParseGenericConstraints(tokens, diagnostics);
        //    var effects = AcceptEffects(tokens, diagnostics);
        //    var contracts = ParseFunctionContracts(tokens, diagnostics);
        //    var body = blockParser.ParseBlock(tokens, diagnostics);
        //    return new InitializerFunctionDeclarationSyntax(modifiers, initKeyword, name,
        //        genericParameters, openParen, parameters, closeParen, genericConstraints, effects, contracts, body);
        //}


        //[MustUseReturnValue]
        //[NotNull]
        //public DestructorFunctionDeclarationSyntax ParseDestructor(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var deleteKeyword = tokens.Take<IDeleteKeywordToken>();
        //    var openParen = tokens.Expect<IOpenParenTokenPlace>();
        //    var parameters = ParseParameterList(tokens, diagnostics);
        //    var closeParen = tokens.Expect<ICloseParenTokenPlace>();
        //    var effects = AcceptEffects(tokens, diagnostics);
        //    var contracts = ParseFunctionContracts(tokens, diagnostics);
        //    var body = blockParser.ParseBlock(tokens, diagnostics);
        //    return new DestructorFunctionDeclarationSyntax(modifiers, deleteKeyword,
        //         openParen, parameters, closeParen, effects, contracts, body);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //private GetterFunctionDeclarationSyntax ParseGetterFunction(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var getKeyword = tokens.Take<IGetKeywordToken>();
        //    var name = tokens.ExpectIdentifier();
        //    var openParen = tokens.Expect<IOpenParenTokenPlace>();
        //    var parameters = ParseParameterList(tokens, diagnostics);
        //    var closeParen = tokens.Expect<ICloseParenTokenPlace>();
        //    var arrow = tokens.Expect<IRightArrowTokenPlace>();
        //    var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
        //    var effects = AcceptEffects(tokens, diagnostics);
        //    var contracts = ParseFunctionContracts(tokens, diagnostics);
        //    var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
        //    return new GetterFunctionDeclarationSyntax(modifiers, getKeyword, name,
        //        openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body, semicolon);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //private SetterFunctionDeclarationSyntax ParseSetterFunction(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var setKeyword = tokens.Take<ISetKeywordToken>();
        //    var name = tokens.ExpectIdentifier();
        //    var openParen = tokens.Expect<IOpenParenTokenPlace>();
        //    var parameters = ParseParameterList(tokens, diagnostics);
        //    var closeParen = tokens.Expect<ICloseParenTokenPlace>();
        //    var arrow = tokens.Expect<IRightArrowTokenPlace>();
        //    var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
        //    var effects = AcceptEffects(tokens, diagnostics);
        //    var contracts = ParseFunctionContracts(tokens, diagnostics);
        //    var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
        //    return new SetterFunctionDeclarationSyntax(modifiers, setKeyword, name,
        //        openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body, semicolon);
        //}

        [MustUseReturnValue]
        [CanBeNull]
        private FixedList<ParameterSyntax> AcceptParameters()
        {
            var parameters = new List<ParameterSyntax>();
            if (Tokens.Accept<IOpenParenToken>() == null)
                return null;
            while (!Tokens.AtEndOfFile() && !(Tokens.Current is ICloseParenToken))
            {
                parameters.Add(parameterParser.ParseParameter(Tokens, Tokens.Context.Diagnostics));
            }
            Tokens.Expect<ICloseParenToken>();
            return parameters.ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        private FixedList<EffectSyntax> ParseMayEffects()
        {
            if (Tokens.Accept<IMayKeywordToken>() == null)
                return FixedList<EffectSyntax>.Empty;

            return listParser.ParseSeparatedList<EffectSyntax, ICommaToken>(ParseEffect);
        }

        [MustUseReturnValue]
        [NotNull]
        private FixedList<EffectSyntax> ParseNoEffects()
        {
            if (Tokens.Accept<INoKeywordToken>() == null)
                return FixedList<EffectSyntax>.Empty;

            return listParser.ParseSeparatedList<EffectSyntax, ICommaToken>(ParseEffect);
        }


        [MustUseReturnValue]
        [NotNull]
        public EffectSyntax ParseEffect()
        {
            switch (Tokens.Current)
            {
                case IThrowKeywordToken throwKeyword:
                    Tokens.Next();
                    var exceptions = listParser.ParseSeparatedList<ThrowEffectEntrySyntax, ICommaToken>(ParseThrowEffectEntry);
                    return new ThrowEffectSyntax(exceptions);
                case IIdentifierToken identifier:
                    Tokens.Next();
                    return new SimpleEffectSyntax(identifier);
                default:
                    throw NonExhaustiveMatchException.For(Tokens.Current);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private ThrowEffectEntrySyntax ParseThrowEffectEntry()
        {
            var paramsKeyword = Tokens.Accept<IParamsKeywordToken>();
            var exceptionType = expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
            return new ThrowEffectEntrySyntax(paramsKeyword, exceptionType);
        }

        [MustUseReturnValue]
        [NotNull]
        private (FixedList<ExpressionSyntax> Requires, FixedList<ExpressionSyntax> Ensures) ParseFunctionContracts()
        {
            var requires = new List<ExpressionSyntax>();
            var ensures = new List<ExpressionSyntax>();
            for (; ; )
                switch (Tokens.Current)
                {
                    case IRequiresKeywordToken _:
                        requires.Add(ParseRequires());
                        break;
                    case IEnsuresKeywordToken _:
                        ensures.Add(ParseEnsures());
                        break;
                    default:
                        return (requires.ToFixedList(), ensures.ToFixedList());
                }
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseRequires()
        {
            Tokens.Required<IRequiresKeywordToken>();
            return expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseEnsures()
        {
            Tokens.Required<IEnsuresKeywordToken>();
            return expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
        }

        private BlockSyntax ParseFunctionBody(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var body = blockParser.AcceptBlock(tokens, diagnostics);
            if (body == null)
                tokens.Expect<ISemicolonToken>();
            return body;
        }
        #endregion

        //[MustUseReturnValue]
        //[NotNull]
        //private ConstDeclarationSyntax ParseConst(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var constKeyword = tokens.Take<IConstKeywordToken>();
        //    var name = tokens.ExpectIdentifier();
        //    IColonTokenPlace colon = null;
        //    ExpressionSyntax typeExpression = null;
        //    if (tokens.Current is IColonToken)
        //    {
        //        colon = tokens.Expect<IColonTokenPlace>();
        //        // Need to not consume the assignment that separates the type from the initializer,
        //        // hence the min operator precedence.
        //        typeExpression = expressionParser.ParseExpression(tokens, diagnostics, OperatorPrecedence.AboveAssignment);
        //    }
        //    IEqualsToken equals = null;
        //    ExpressionSyntax initializer = null;
        //    if (tokens.Current is IEqualsToken)
        //    {
        //        equals = tokens.Take<IEqualsToken>();
        //        initializer = expressionParser.ParseExpression(tokens, diagnostics);
        //    }
        //    var semicolon = tokens.Expect<ISemicolonTokenPlace>();
        //    return new ConstDeclarationSyntax(attributes, modifiers, constKeyword, name, colon, typeExpression,
        //        equals, initializer, semicolon);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //private static IncompleteDeclarationSyntax ParseIncompleteDeclaration(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var skipped = attributes.SelectMany(a => a.Tokens()).Concat(modifiers.SelectMany(m => m.Tokens())).ToList();
        //    skipped.Add(tokens.ExpectIdentifier());    // The name we are expecting

        //    if (skipped.All(s => s is IMissingToken))
        //    {
        //        // We haven't consumed any tokens, we need to consume a token so
        //        // we make progress.
        //        skipped.Add(tokens.Current);
        //        tokens.Next();
        //    }

        //    var span = TextSpan.Covering(
        //        skipped.First(s => s != null).NotNull().Span,
        //        skipped.Last(s => s != null).NotNull().Span);

        //    diagnostics.Add(ParseError.IncompleteDeclaration(tokens.Context.File, span));
        //    return new IncompleteDeclarationSyntax(skipped);
        //}
    }
}
