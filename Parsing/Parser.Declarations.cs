using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        public FixedList<DeclarationSyntax> ParseDeclarations()
        {
            var declarations = new List<DeclarationSyntax>();
            // Stop at end of file or close brace that contains these declarations
            while (!Tokens.AtEnd<ICloseBraceToken>())
                try
                {
                    declarations.Add(ParseDeclaration());
                }
                catch (ParseFailedException)
                {
                    // Ignore: we would have consumed something before failing, try to get the next declaration
                }

            return declarations.ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        public DeclarationSyntax ParseDeclaration()
        {
            var attributes = ParseAttributes();
            var modifiers = AcceptMany(Tokens.AcceptToken<IModiferToken>);

            switch (Tokens.Current)
            {
                case INamespaceKeywordToken _:
                    return ParseNamespaceDeclaration(attributes, modifiers);
                case IClassKeywordToken _:
                    return ParseClass(attributes, modifiers);
                case ITraitKeywordToken _:
                    return ParseTrait(attributes, modifiers);
                case IStructKeywordToken _:
                    return ParseStruct(attributes, modifiers);
                case IEnumKeywordToken _:
                    return ParseEnum(attributes, modifiers);
                case IFunctionKeywordToken _:
                    return ParseNamedFunction(attributes, modifiers);
                case IConstKeywordToken _:
                    return ParseConst(attributes, modifiers);
                default:
                    Tokens.UnexpectedToken();
                    throw new ParseFailedException();
            }
        }

        /// <summary>
        /// Skip tokens until we reach what we assume to be the end of a statement
        /// </summary>
        // TODO does this belong on a statement parser or something
        private void SkipToEndOfStatement()
        {
            while (!Tokens.AtEnd<ISemicolonToken>())
            {
                Tokens.Next();
            }
            // Consume the semicolon is we aren't at the end of the file.
            var _ = Tokens.Accept<ISemicolonToken>();
        }

        [MustUseReturnValue]
        [NotNull]
        public IMemberDeclarationSyntax ParseMemberDeclaration()
        {
            var attributes = ParseAttributes();
            var modifiers = AcceptMany(Tokens.AcceptToken<IModiferToken>);

            switch (Tokens.Current)
            {
                case IClassKeywordToken _:
                    return ParseClass(attributes, modifiers);
                case ITraitKeywordToken _:
                    return ParseTrait(attributes, modifiers);
                case IStructKeywordToken _:
                    return ParseStruct(attributes, modifiers);
                case IEnumKeywordToken _:
                    return ParseEnum(attributes, modifiers);
                case IFunctionKeywordToken _:
                    return ParseNamedFunction(attributes, modifiers);
                case IOperatorKeywordToken _:
                    return ParseOperator(attributes, modifiers);
                case INewKeywordToken _:
                    return ParseConstructor(attributes, modifiers);
                case IInitKeywordToken _:
                    return ParseInitializer(attributes, modifiers);
                case IDeleteKeywordToken _:
                    return ParseDestructor(attributes, modifiers);
                case IGetKeywordToken _:
                    return ParseGetter(attributes, modifiers);
                case ISetKeywordToken _:
                    return ParseSetter(attributes, modifiers);
                case IVarKeywordToken _:
                case ILetKeywordToken _:
                    return ParseField(attributes, modifiers);
                case IConstKeywordToken _:
                    return ParseConst(attributes, modifiers);
                default:
                    Tokens.UnexpectedToken();
                    throw new ParseFailedException();
            }
        }

        #region Parse Namespaces
        [MustUseReturnValue]
        [NotNull]
        public NamespaceDeclarationSyntax ParseNamespaceDeclaration(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            // TODO generate errors for attributes or modifiers
            Tokens.Expect<INamespaceKeywordToken>();
            var globalQualifier = Tokens.AcceptToken<IColonColonToken>();
            var (name, span) = ParseNamespaceName();
            span = TextSpan.Covering(span, globalQualifier?.Span);
            Tokens.Expect<IOpenBraceToken>();
            var bodyParser = NestedParser(nameContext.Qualify(name));
            var usingDirectives = bodyParser.ParseUsingDirectives();
            var declarations = bodyParser.ParseDeclarations();
            Tokens.Expect<ICloseBraceToken>();
            return new NamespaceDeclarationSyntax(File, globalQualifier != null, name,
                span, nameContext, usingDirectives, declarations);
        }

        [MustUseReturnValue]
        private (Name, TextSpan) ParseNamespaceName()
        {
            var nameSegment = Tokens.RequiredToken<IIdentifierToken>();
            var span = nameSegment.Span;
            Name name = new SimpleName(nameSegment.Value);

            while (Tokens.Accept<IDotToken>())
            {
                nameSegment = Tokens.ExpectIdentifier();
                if (nameSegment == null) break;
                span = TextSpan.Covering(span, nameSegment.Span);
                name = name.Qualify(nameSegment.Value);
            }

            return (name, span);
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
            if (!Tokens.Accept<IHashHashToken>()) return null;
            var name = ParseName();
            FixedList<ArgumentSyntax> arguments = null;
            if (Tokens.Accept<IOpenParenToken>())
            {
                arguments = ParseArguments();
                Tokens.Expect<ICloseParenToken>();
            }
            return new AttributeSyntax(name, arguments);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private ExpressionSyntax AcceptBaseClass()
        {
            if (!Tokens.Accept<IColonToken>()) return null;
            return ParseExpression();
        }

        [MustUseReturnValue]
        [CanBeNull]
        private FixedList<ExpressionSyntax> AcceptBaseTypes()
        {
            if (!Tokens.Accept<ILessThanColonToken>()) return null;
            return AcceptOneOrMore<ExpressionSyntax, ICommaToken>(ParseExpression);
        }

        [MustUseReturnValue]
        [NotNull]
        private FixedList<ExpressionSyntax> ParseInvariants()
        {
            return AcceptMany(AcceptInvariant);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private ExpressionSyntax AcceptInvariant()
        {
            if (!Tokens.Accept<IInvariantKeywordToken>()) return null;
            return ParseExpression();
        }

        [NotNull]
        private FixedList<IMemberDeclarationSyntax> ParseMemberDeclarations()
        {
            return ParseMany<IMemberDeclarationSyntax, ICloseBraceToken>(ParseMemberDeclaration);
        }

        [NotNull]
        private FixedList<IMemberDeclarationSyntax> ParseTypeBody()
        {
            Tokens.Expect<IOpenBraceToken>();
            var members = ParseMemberDeclarations();
            Tokens.Expect<ICloseBraceToken>();
            return members;
        }
        #endregion

        #region Parse Type Declarations
        [MustUseReturnValue]
        [NotNull]
        private ClassDeclarationSyntax ParseClass(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            Tokens.Expect<IClassKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var genericParameters = AcceptGenericParameters();
            var baseClass = AcceptBaseClass();
            var baseTypes = AcceptBaseTypes();
            var genericConstraints = ParseGenericConstraints();
            var invariants = ParseInvariants();
            var members = ParseTypeBody();
            return new ClassDeclarationSyntax(File, attributes, modifiers, name, identifier.Span, genericParameters,
                baseClass, baseTypes, genericConstraints, invariants,
                members);
        }

        [MustUseReturnValue]
        [NotNull]
        private TraitDeclarationSyntax ParseTrait(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            Tokens.Expect<ITraitKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var genericParameters = AcceptGenericParameters();
            var baseTypes = AcceptBaseTypes();
            var genericConstraints = ParseGenericConstraints();
            var invariants = ParseInvariants();
            var members = ParseTypeBody();
            return new TraitDeclarationSyntax(File, attributes, modifiers, name, identifier.Span,
                genericParameters, baseTypes, genericConstraints, invariants,
                members);
        }

        [MustUseReturnValue]
        [NotNull]
        private StructDeclarationSyntax ParseStruct(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            Tokens.Expect<IStructKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var genericParameters = AcceptGenericParameters();
            var baseTypes = AcceptBaseTypes();
            var genericConstraints = ParseGenericConstraints();
            var invariants = ParseInvariants();
            var members = ParseTypeBody();
            return new StructDeclarationSyntax(File, attributes, modifiers, name, identifier.Span, genericParameters,
                baseTypes, genericConstraints, invariants,
                members);
        }

        [MustUseReturnValue]
        [NotNull]
        private TypeDeclarationSyntax ParseEnum(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            Tokens.Expect<IEnumKeywordToken>();
            switch (Tokens.Current)
            {
                case IStructKeywordToken _:
                {
                    Tokens.Expect<IStructKeywordToken>();
                    var identifier = Tokens.RequiredToken<IIdentifierToken>();
                    var name = nameContext.Qualify(identifier.Value);
                    var genericParameters = AcceptGenericParameters();
                    var baseTypes = AcceptBaseTypes();
                    var genericConstraints = ParseGenericConstraints();
                    var invariants = ParseInvariants();
                    Tokens.Expect<IOpenBraceToken>();
                    var variants = ParseEnumVariants();
                    var members = ParseMemberDeclarations();
                    Tokens.Expect<ICloseBraceToken>();
                    return new EnumStructDeclarationSyntax(File, attributes, modifiers,
                        name, identifier.Span, genericParameters, baseTypes, genericConstraints,
                        invariants, variants, members);
                }
                case IClassKeywordToken _:
                {
                    Tokens.Expect<IClassKeywordToken>();
                    var identifier = Tokens.RequiredToken<IIdentifierToken>();
                    var name = nameContext.Qualify(identifier.Value);
                    var genericParameters = AcceptGenericParameters();
                    var baseClass = AcceptBaseClass();
                    var baseTypes = AcceptBaseTypes();
                    var genericConstraints = ParseGenericConstraints();
                    var invariants = ParseInvariants();
                    Tokens.Expect<IOpenBraceToken>();
                    var variants = ParseEnumVariants();
                    var members = ParseMemberDeclarations();
                    Tokens.Expect<ICloseBraceToken>();
                    return new EnumClassDeclarationSyntax(File, attributes, modifiers,
                        name, identifier.Span, genericParameters, baseClass, baseTypes, genericConstraints,
                        invariants, variants, members);
                }
                default:
                    throw new ParseFailedException();
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private FixedList<EnumVariantSyntax> ParseEnumVariants()
        {
            var variants = new List<EnumVariantSyntax>();
            while (Tokens.Current is IIdentifierToken)
                variants.Add(ParseEnumVariant());

            // Semicolon is optional if there are no members
            if (!Tokens.AtEnd<ICloseBraceToken>())
                Tokens.Expect<ISemicolonToken>();

            return variants.ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        private EnumVariantSyntax ParseEnumVariant()
        {
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            // TODO we actually expect commas separating them, need to improve this
            var comma = Tokens.AcceptToken<ICommaToken>();
            return new EnumVariantSyntax(identifier.Value);
        }
        #endregion

        #region Parse Type Member Declarations
        [MustUseReturnValue]
        [NotNull]
        private FieldDeclarationSyntax ParseField(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            // TODO include these in the syntax tree
            var binding = Tokens.Expect<IBindingToken>();
            var getterAccess = AcceptFieldGetter();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            ExpressionSyntax typeExpression = null;
            if (Tokens.Accept<IColonToken>())
            {
                // Need to not consume the assignment that separates the type from the initializer,
                // hence the min operator precedence.
                typeExpression = ParseExpression(OperatorPrecedence.AboveAssignment);
            }
            ExpressionSyntax initializer = null;
            if (Tokens.Accept<IEqualsToken>())
                initializer = ParseExpression();

            Tokens.Expect<ISemicolonToken>();
            return new FieldDeclarationSyntax(File, attributes, modifiers, getterAccess, name, identifier.Span, typeExpression, initializer);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private AccessModifier? AcceptFieldGetter()
        {
            // TODO allow other access modifiers
            var publicKeyword = Tokens.AcceptToken<IPublicKeywordToken>();
            var getKeyword = Tokens.AcceptToken<IGetKeywordToken>();
            if (publicKeyword == null && getKeyword == null) return null;
            return AccessModifier.Public;
        }
        #endregion

        #region Parse Functions
        [MustUseReturnValue]
        [NotNull]
        public NamedFunctionDeclarationSyntax ParseNamedFunction(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            Tokens.Expect<IFunctionKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var genericParameters = AcceptGenericParameters();
            var name = nameContext.Qualify(identifier.Value);
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters();
            ExpressionSyntax returnType = null;
            if (Tokens.Accept<IRightArrowToken>())
                returnType = ParseExpression();
            var genericConstraints = ParseGenericConstraints();
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = bodyParser.ParseFunctionBody();
            return new NamedFunctionDeclarationSyntax(File, modifiers, name, identifier.Span, genericParameters, parameters, returnType,
                genericConstraints, mayEffects, noEffects, requires, ensures, body);
        }

        [MustUseReturnValue]
        [NotNull]
        public OperatorDeclarationSyntax ParseOperator(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var operatorKeyword = Tokens.Expect<IOperatorKeywordToken>();
            var genericParameters = AcceptGenericParameters();
            // TODO correctly store these in the syntax class
            OverloadableOperator oper;
            TextSpan endOperatorSpan;
            switch (Tokens.Current)
            {
                case IHashToken _:
                    Tokens.Expect<IHashToken>();
                    switch (Tokens.Current)
                    {
                        case IOpenParenToken _:
                            Tokens.Next();
                            endOperatorSpan = Tokens.Expect<ICloseParenToken>();
                            oper = OverloadableOperator.TupleInitializer;
                            break;
                        case IOpenBracketToken _:
                            Tokens.Next();
                            endOperatorSpan = Tokens.Expect<ICloseBracketToken>();
                            oper = OverloadableOperator.ListInitializer;
                            break;
                        case IOpenBraceToken _:
                            Tokens.Next();
                            endOperatorSpan = Tokens.Expect<ICloseBraceToken>();
                            oper = OverloadableOperator.SetInitializer;
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(Tokens.Current);
                    }
                    break;
                case IStringLiteralToken _:
                    endOperatorSpan = Tokens.Expect<IStringLiteralToken>();
                    // TODO need to check it is empty string
                    oper = OverloadableOperator.StringLiteral;
                    break;
                // TODO case for user defined literals ''
                default:
                    Tokens.UnexpectedToken();
                    throw new ParseFailedException();
            }

            var nameSpan = TextSpan.Covering(operatorKeyword, endOperatorSpan);
            var name = nameContext.Qualify(SimpleName.Special("operator_" + oper));
            var parameters = ParseParameters();
            ExpressionSyntax returnType = null;
            if (Tokens.Accept<IRightArrowToken>())
                returnType = ParseExpression();
            var genericConstraints = ParseGenericConstraints();
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = ParseBlock();
            return new OperatorDeclarationSyntax(File, modifiers, name, nameSpan, genericParameters,
                 parameters, returnType, genericConstraints, mayEffects, noEffects, requires, ensures, body);
        }

        [MustUseReturnValue]
        [NotNull]
        public ConstructorDeclarationSyntax ParseConstructor(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var newKeywordSpan = Tokens.Expect<INewKeywordToken>();
            var identifier = Tokens.AcceptToken<IIdentifierToken>();
            var name = nameContext.Qualify(SimpleName.Special("new" + (identifier != null ? "_" + identifier.Value : "")));
            var bodyParser = NestedParser(name);
            var genericParameters = AcceptGenericParameters();
            var parameters = bodyParser.ParseParameters();
            var genericConstraints = ParseGenericConstraints();
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = bodyParser.ParseBlock();
            return new ConstructorDeclarationSyntax(File, modifiers, name, TextSpan.Covering(newKeywordSpan, identifier?.Span),
                genericParameters, parameters, genericConstraints, mayEffects, noEffects, requires, ensures, body);
        }

        [MustUseReturnValue]
        [NotNull]
        public InitializerDeclarationSyntax ParseInitializer(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var initKeywordSpan = Tokens.Expect<IInitKeywordToken>();
            var identifier = Tokens.AcceptToken<IIdentifierToken>();
            var name = nameContext.Qualify(SimpleName.Special("init" + (identifier != null ? "_" + identifier.Value : "")));
            var bodyParser = NestedParser(name);
            var genericParameters = AcceptGenericParameters();
            var parameters = bodyParser.ParseParameters();
            var genericConstraints = ParseGenericConstraints();
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = bodyParser.ParseBlock();
            return new InitializerDeclarationSyntax(File, modifiers, name, TextSpan.Covering(initKeywordSpan, identifier?.Span),
                genericParameters, parameters, genericConstraints, mayEffects, noEffects, requires, ensures, body);
        }


        [MustUseReturnValue]
        [NotNull]
        public DestructorDeclarationSyntax ParseDestructor(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var deleteKeywordSpan = Tokens.Expect<IDeleteKeywordToken>();
            var name = nameContext.Qualify(SpecialName.Delete);
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters();
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = bodyParser.ParseBlock();
            return new DestructorDeclarationSyntax(File, modifiers, name, deleteKeywordSpan,
                 parameters, mayEffects, noEffects, requires, ensures, body);
        }

        [MustUseReturnValue]
        [NotNull]
        private GetterDeclarationSyntax ParseGetter(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            Tokens.Expect<IGetKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var propertyName = nameContext.Qualify(identifier.Value);
            var name = nameContext.Qualify(SimpleName.Special("get_" + identifier.Value));
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters();
            Tokens.Expect<IRightArrowToken>();
            var returnType = ParseExpression();
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = bodyParser.ParseBlock();
            return new GetterDeclarationSyntax(File, attributes, modifiers, propertyName, name, identifier.Span,
                parameters, returnType, mayEffects, noEffects, requires, ensures, body);
        }

        [MustUseReturnValue]
        [NotNull]
        private SetterDeclarationSyntax ParseSetter(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            Tokens.Expect<ISetKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var propertyName = nameContext.Qualify(identifier.Value);
            var name = nameContext.Qualify(SimpleName.Special("get_" + identifier.Value));
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters();
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = bodyParser.ParseBlock();
            return new SetterDeclarationSyntax(File, attributes, modifiers, propertyName, name, identifier.Span,
                 parameters, mayEffects, noEffects, requires, ensures, body);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private FixedList<ParameterSyntax> AcceptParameters()
        {
            if (!Tokens.Accept<IOpenParenToken>())
                return null;
            return ParseRestOfParameters();
        }

        [MustUseReturnValue]
        [NotNull]
        private FixedList<ParameterSyntax> ParseParameters()
        {
            Tokens.Expect<IOpenParenToken>();
            return ParseRestOfParameters();
        }

        /// <summary>
        /// Requires that the open paren has already been consumed
        /// </summary>
        /// <returns></returns>
        [NotNull]
        [MustUseReturnValue]
        private FixedList<ParameterSyntax> ParseRestOfParameters()
        {
            var parameters = new List<ParameterSyntax>();
            while (!Tokens.AtEnd<ICloseParenToken>())
            {
                parameters.Add(ParseParameter());
                if (Tokens.Current is ICommaToken)
                    Tokens.Expect<ICommaToken>();
            }

            Tokens.Expect<ICloseParenToken>();
            return parameters.ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        private FixedList<EffectSyntax> ParseMayEffects()
        {
            if (!Tokens.Accept<IMayKeywordToken>())
                return FixedList<EffectSyntax>.Empty;

            return AcceptOneOrMore<EffectSyntax, ICommaToken>(ParseEffect);
        }

        [MustUseReturnValue]
        [NotNull]
        private FixedList<EffectSyntax> ParseNoEffects()
        {
            if (!Tokens.Accept<INoKeywordToken>())
                return FixedList<EffectSyntax>.Empty;

            return AcceptOneOrMore<EffectSyntax, ICommaToken>(ParseEffect);
        }

        [MustUseReturnValue]
        [NotNull]
        public EffectSyntax ParseEffect()
        {
            switch (Tokens.Current)
            {
                case IThrowKeywordToken _:
                    Tokens.Expect<IThrowKeywordToken>();
                    var exceptions = AcceptOneOrMore<ThrowEffectEntrySyntax, ICommaToken>(ParseThrowEffectEntry);
                    return new ThrowEffectSyntax(exceptions);
                case IIdentifierToken _:
                    return new SimpleEffectSyntax(Tokens.RequiredToken<IIdentifierToken>().Value);
                default:
                    Tokens.UnexpectedToken();
                    throw new ParseFailedException();
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private ThrowEffectEntrySyntax ParseThrowEffectEntry()
        {
            var isParams = Tokens.Accept<IParamsKeywordToken>();
            var exceptionType = ParseExpression();
            return new ThrowEffectEntrySyntax(isParams, exceptionType);
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
            Tokens.Expect<IRequiresKeywordToken>();
            return ParseExpression();
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseEnsures()
        {
            Tokens.Expect<IEnsuresKeywordToken>();
            return ParseExpression();
        }

        private BlockSyntax ParseFunctionBody()
        {
            var body = AcceptBlock();
            if (body == null)
                Tokens.Expect<ISemicolonToken>();
            return body;
        }
        #endregion

        [MustUseReturnValue]
        [NotNull]
        private ConstDeclarationSyntax ParseConst(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            try
            {
                Tokens.Expect<IConstKeywordToken>();
                var identifier = Tokens.RequiredToken<IIdentifierToken>();
                var name = nameContext.Qualify(identifier.Value);
                ExpressionSyntax type = null;
                if (Tokens.Accept<IColonToken>())
                {
                    // Need to not consume the assignment that separates the type from the initializer,
                    // hence the min operator precedence.
                    type = ParseExpression(OperatorPrecedence.AboveAssignment);
                }

                ExpressionSyntax initializer = null;
                if (Tokens.Accept<IEqualsToken>())
                    initializer = ParseExpression();

                Tokens.Expect<ISemicolonToken>();
                return new ConstDeclarationSyntax(File, attributes, modifiers, name, identifier.Span, type, initializer);
            }
            catch (ParseFailedException)
            {
                SkipToEndOfStatement();
                throw;
            }
        }
    }
}
