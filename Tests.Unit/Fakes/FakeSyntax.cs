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
        public static IIdentifierToken Identifier([NotNull] string name)
        {
            return TokenFactory.BareIdentifier(FakeSpan, name);
        }


    }
}
