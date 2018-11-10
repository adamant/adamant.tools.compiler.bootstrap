using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes
{
    // A factory for fakes
    public static class FakeSyntax
    {
        private static readonly TextSpan FakeSpan = new TextSpan(0, 0);

        [NotNull]
        private static T Missing<T>()
            where T : ITokenPlace
        {
            return (T)TokenFactory.Missing(FakeSpan);
        }

        [NotNull]
        public static AccessModifierSyntax AccessModifier()
        {
            return new AccessModifierSyntax(Missing<IAccessModifierTokenPlace>());
        }

        [NotNull]
        public static BlockSyntax Block()
        {
            return new BlockSyntax(Missing<IOpenBraceTokenPlace>(), List<StatementSyntax>(), Missing<ICloseBraceTokenPlace>());
        }

        [NotNull]
        public static SyntaxList<T> List<T>()
            where T : NonTerminal
        {
            return new SyntaxList<T>(Enumerable.Empty<T>());
        }

        [NotNull]
        public static ExpressionSyntax Expression()
        {
            return new FakeExpressionSyntax();
        }

        [NotNull]
        public static SeparatedListSyntax<T> SeparatedList<T>()
            where T : NonTerminal
        {
            return new SeparatedListSyntax<T>(Enumerable.Empty<T>());
        }

        [NotNull]
        public static NameSyntax Name()
        {
            return new FakeNameSyntax();
        }

        [NotNull]
        public static NameSyntax Name([NotNull] string name)
        {
            var parts = name.Split('.').NotNull();

            NameSyntax nameSyntax = null;
            foreach (var part in parts)
            {
                var partSyntax = new IdentifierNameSyntax(Identifier(part));
                if (nameSyntax == null)
                    nameSyntax = partSyntax;
                else
                    nameSyntax = new QualifiedNameSyntax(nameSyntax, TokenFactory.Dot(FakeSpan), partSyntax);
            }

            return nameSyntax.NotNull();
        }

        [NotNull]
        public static UsingDirectiveSyntax UsingDirective()
        {
            return new UsingDirectiveSyntax(Missing<IUsingKeywordTokenPlace>(), Name(), Missing<ISemicolonTokenPlace>());
        }

        public static ParameterSyntax Parameter()
        {
            return new NamedParameterSyntax(null, null, Missing<IIdentifierTokenPlace>(),
                Missing<IColonTokenPlace>(), Expression(), null, null);
        }

        private class FakeExpressionSyntax : ExpressionSyntax
        {
            public FakeExpressionSyntax()
                : base(FakeSpan)
            {
            }
        }

        private class FakeNameSyntax : NameSyntax
        {
            public FakeNameSyntax()
                : base(FakeSpan)
            {
            }
        }

        [NotNull]
        public static IncompleteDeclarationSyntax IncompleteDeclaration()
        {
            // Need at least one token to prevent issues computing the span of the incomplete declaration
            return new IncompleteDeclarationSyntax(new[] { Missing<IPublicKeywordTokenPlace>() });
        }

        [NotNull]
        public static CompilationUnitSyntax CompilationUnit(
            [NotNull] params DeclarationSyntax[] declarations)
        {
            return CompilationUnit(null, declarations);
        }

        [NotNull]
        public static CompilationUnitSyntax CompilationUnit(
            [CanBeNull] NameSyntax @namespace,
            [NotNull] params DeclarationSyntax[] declarations)
        {
            var namespaceSyntax = new FileNamespaceDeclarationSyntax(
                null, @namespace, null, SyntaxList<UsingDirectiveSyntax>.Empty,
                new SyntaxList<DeclarationSyntax>(declarations)); ;
            return new CompilationUnitSyntax(
                FakeCodeFile.For(""),
                namespaceSyntax,
                FixedList<Diagnostic>.Empty);
        }

        [NotNull]
        public static PackageSyntax Package([NotNull]params CompilationUnitSyntax[] compilationUnits)
        {
            return new PackageSyntax("Test", new SyntaxList<CompilationUnitSyntax>(compilationUnits));
        }

        [NotNull]
        public static NamedFunctionDeclarationSyntax FunctionDeclaration([NotNull] string name)
        {
            return new NamedFunctionDeclarationSyntax(
                AccessModifier().ToSyntaxList<ModifierSyntax>(),
                TokenFactory.FunctionKeyword(FakeSpan),
                Identifier(name),
                null, // Generic Parameters
                Missing<IOpenParenTokenPlace>(),
                SeparatedListSyntax<ParameterSyntax>.Empty,
                Missing<ICloseParenTokenPlace>(),
                Missing<IRightArrowTokenPlace>(),
                Expression(),
                SyntaxList<GenericConstraintSyntax>.Empty,
                null, // Effects
                SyntaxList<FunctionContractSyntax>.Empty,
                Block(),
                null); // Semicolon
        }


        [NotNull]
        public static IIdentifierToken Identifier([NotNull] string name)
        {
            return TokenFactory.BareIdentifier(FakeSpan, name);
        }

        [NotNull]
        public static EnumStructDeclarationSyntax EnumStructDeclaration([NotNull] string name)
        {
            return new EnumStructDeclarationSyntax(
                AccessModifier().ToSyntaxList<ModifierSyntax>(),
                TokenFactory.EnumKeyword(FakeSpan),
                TokenFactory.StructKeyword(FakeSpan),
                Identifier(name),
                null, // Generic Parameters
                null, // Base Types
                SyntaxList<GenericConstraintSyntax>.Empty,
                SyntaxList<InvariantSyntax>.Empty,
                Missing<IOpenBraceTokenPlace>(),
                new EnumVariantsSyntax(SyntaxList<EnumVariantSyntax>.Empty, null),
                SyntaxList<MemberDeclarationSyntax>.Empty,
                Missing<ICloseBraceTokenPlace>());
        }

        [NotNull]
        public static GenericParametersSyntax GenericParameters()
        {
            return new GenericParametersSyntax(
                Missing<IOpenBracketTokenPlace>(),
                SeparatedListSyntax<GenericParameterSyntax>.Empty,
                Missing<ICloseBracketTokenPlace>());
        }
    }
}
