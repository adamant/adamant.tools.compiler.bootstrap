using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Helpers;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Parsing.Fakes
{
    public static class FakeParser
    {
        [NotNull]
        public static IListParser ForLists()
        {
            return new ListParser();
        }

        [NotNull]
        public static IExpressionParser ForExpressions()
        {
            return new ExpressionParser();
        }

        [NotNull]
        public static INameParser ForNames()
        {
            return new NameParser();
        }

        [NotNull]
        public static IDeclarationParser ForDeclarations()
        {
            return new DeclarationParser();
        }

        [NotNull]
        public static IModifierParser ForModifiers()
        {
            return new ModifierParser();
        }

        [NotNull]
        public static IParameterParser ForParameters()
        {
            return new ParameterParser();
        }

        [NotNull]
        public static IUsingDirectiveParser ForUsingDirectives()
        {
            return new UsingDirectiveParser();
        }

        [NotNull]
        public static IBlockParser ForBlocks()
        {
            return new BlockParser();
        }

        [NotNull]
        public static IGenericsParser ForGenerics()
        {
            return new GenericsParser();
        }

        [NotNull]
        public static SkipParser<T> Skip<T>(T value)
            where T : NonTerminal
        {
            return new SkipParser<T>(value);
        }

        [NotNull]
        private static T FakeParse<T>(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
            where T : class
        {
            var fakeToken = tokens.ExpectFake();
            return (T)fakeToken?.FakeValue ?? throw new Exception($"Expected fake '{typeof(T).Name}' not found");
        }

        public class SkipParser<T>
            where T : NonTerminal
        {
            private T value;

            public SkipParser(T value)
            {
                this.value = value;
            }

            public T Parse([NotNull] ITokenIterator tokens, [NotNull] Diagnostics diagnostics)
            {
                var v = value;
                value = null;
                return v;
            }
        }

        private class ListParser : IListParser
        {
            [NotNull]
            public FixedList<T> ParseList<T>([NotNull] Func<T> acceptItem)
                where T : class
            {
                throw new NotImplementedException();
            }

            public SyntaxList<T> ParseList<T>(
                ITokenIterator tokens,
                AcceptFunction<T> acceptItem,
                Diagnostics diagnostics) where T : NonTerminal
            {
                var fakeToken = tokens.ExpectFake();
                return (SyntaxList<T>)fakeToken?.FakeValue ?? throw new InvalidOperationException();
            }

            public SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator>(
                ITokenIterator tokens,
                AcceptFunction<T> acceptItem,
                Type<TSeparator> separatorType,
                Diagnostics diagnostics)
                where T : NonTerminal
                where TSeparator : class, IToken
            {
                var fakeToken = tokens.ExpectFake();
                return (SeparatedListSyntax<T>)fakeToken?.FakeValue ?? throw new InvalidOperationException();
            }

            public SyntaxList<T> ParseList<T, TTerminator>(
                ITokenIterator tokens,
                ParseFunction<T> parseItem,
                Type<TTerminator> terminatorType,
                Diagnostics diagnostics)
                where T : NonTerminal
                where TTerminator : class, IToken
            {
                var fakeToken = tokens.ExpectFake();
                return (SyntaxList<T>)fakeToken?.FakeValue ?? throw new InvalidOperationException();
            }

            public SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator, TTerminator>(
                ITokenIterator tokens,
                ParseFunction<T> parseItem,
                Type<TSeparator> separatorType,
                Type<TTerminator> terminatorType,
                Diagnostics diagnostics)
                where T : NonTerminal
                where TSeparator : class, IToken
                where TTerminator : class, IToken
            {
                var fakeToken = tokens.ExpectFake();
                return (SeparatedListSyntax<T>)fakeToken?.FakeValue ?? throw new InvalidOperationException();
            }
        }

        private class ExpressionParser : IExpressionParser
        {
            public ExpressionSyntax ParseExpression(ITokenIterator tokens, Diagnostics diagnostics)
            {
                return FakeParse<ExpressionSyntax>(tokens, diagnostics);
            }

            public ExpressionSyntax ParseExpression(
                ITokenIterator tokens,
                Diagnostics diagnostics,
                OperatorPrecedence minPrecedence)
            {
                return FakeParse<ExpressionSyntax>(tokens, diagnostics);
            }

            public SeparatedListSyntax<ArgumentSyntax> ParseArgumentList(ITokenIterator tokens, Diagnostics diagnostics)
            {
                return SeparatedListSyntax<ArgumentSyntax>.Empty;
            }
        }

        private class NameParser : INameParser
        {
            public NameSyntax ParseName(ITokenIterator tokens, Diagnostics diagnostics)
            {
                return FakeParse<NameSyntax>(tokens, diagnostics);
            }
        }

        private class DeclarationParser : IDeclarationParser
        {
            public FixedList<DeclarationSyntax> ParseDeclarations()
            {
                throw new NotImplementedException();
                //return FakeParse<FixedList<DeclarationSyntax>>(tokens, diagnostics);
            }
        }

        private class ParameterParser : IParameterParser
        {
            public ParameterSyntax ParseParameter([NotNull] ITokenIterator tokens, [NotNull] Diagnostics diagnostics)
            {
                return FakeParse<ParameterSyntax>(tokens, diagnostics);
            }
        }

        private class BlockParser : IBlockParser
        {
            public BlockSyntax AcceptBlock(ITokenIterator tokens, Diagnostics diagnostics)
            {
                return FakeParse<BlockSyntax>(tokens, diagnostics);
            }

            public BlockSyntax ParseBlock([NotNull] ITokenIterator tokens, [NotNull] Diagnostics diagnostics)
            {
                return FakeParse<BlockSyntax>(tokens, diagnostics);
            }
        }

        private class GenericsParser : IGenericsParser
        {
            public GenericParametersSyntax AcceptGenericParameters(
                ITokenIterator tokens,
                Diagnostics diagnostics)
            {
                return FakeParse<GenericParametersSyntax>(tokens, diagnostics);
            }

            public SyntaxList<GenericConstraintSyntax> ParseGenericConstraints(ITokenIterator tokens, Diagnostics diagnostics)
            {
                return SyntaxList<GenericConstraintSyntax>.Empty;
            }
        }

        private class UsingDirectiveParser : IUsingDirectiveParser
        {
            public SyntaxList<UsingDirectiveSyntax> ParseUsingDirectives(ITokenIterator tokens, Diagnostics diagnostics)
            {
                return FakeParse<SyntaxList<UsingDirectiveSyntax>>(tokens, diagnostics);
            }

            //[NotNull]
            //public UsingDirectiveSyntax ParseUsingDirective(
            //    [NotNull] ITokenStream tokens,
            //    IDiagnosticsCollector diagnostics)
            //{
            //    var _ = tokens.Take<UsingKeywordToken>();

            //    var fakeToken = tokens.ExpectFake();
            //    return (UsingDirectiveSyntax)fakeToken?.FakeValue ?? throw new InvalidOperationException();
            //}
        }

        private class ModifierParser : IModifierParser
        {
            public ModifierSyntax AcceptModifier(ITokenIterator tokens, Diagnostics diagnostics)
            {
                if (tokens.Current is FakeToken fake && fake.FakeValue is ModifierSyntax syntax)
                {
                    tokens.Next();
                    return syntax;
                }

                return null;
            }
        }
    }
}
