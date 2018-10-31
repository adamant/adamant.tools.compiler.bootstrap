using System;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Parameters;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Helpers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes
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
        public static SkipParser<T> Skip<T>(T value)
            where T : SyntaxNode
        {
            return new SkipParser<T>(value);
        }

        [NotNull]
        private static T FakeParse<T>(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : SyntaxNode
        {
            var fakeToken = tokens.ExpectFake();
            return (T)fakeToken?.FakeValue ?? throw new Exception($"Expected fake '{typeof(T).Name}' not found");
        }

        public class SkipParser<T>
            where T : SyntaxNode
        {
            private T value;

            public SkipParser(T value)
            {
                this.value = value;
            }

            public T Parse([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
            {
                var v = value;
                value = null;
                return v;
            }
        }

        private class ListParser : IListParser
        {
            public SyntaxList<T> ParseList<T, TTerminator>(
                ITokenStream tokens,
                ParseFunction<T> parseItem,
                Type<TTerminator> terminatorType,
                IDiagnosticsCollector diagnostics) where T : SyntaxNode where TTerminator : Token
            {
                var fakeToken = tokens.ExpectFake();
                return (SyntaxList<T>)fakeToken?.FakeValue ?? throw new InvalidOperationException();
            }

            public SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator, TTerminator>(
                ITokenStream tokens,
                ParseFunction<T> parseItem,
                Type<TSeparator> separatorType,
                Type<TTerminator> terminatorType,
                IDiagnosticsCollector diagnostics) where T : SyntaxNode where TSeparator : Token where TTerminator : Token
            {
                var fakeToken = tokens.ExpectFake();
                return (SeparatedListSyntax<T>)fakeToken?.FakeValue ?? throw new InvalidOperationException();
            }
        }

        private class ExpressionParser : IExpressionParser
        {
            public ExpressionSyntax ParseExpression(ITokenStream tokens, IDiagnosticsCollector diagnostics)
            {
                return FakeParse<ExpressionSyntax>(tokens, diagnostics);
            }

            public ExpressionSyntax ParseExpression(
                ITokenStream tokens,
                IDiagnosticsCollector diagnostics,
                OperatorPrecedence minPrecedence)
            {
                return FakeParse<ExpressionSyntax>(tokens, diagnostics);
            }
        }

        private class NameParser : INameParser
        {
            public NameSyntax ParseName(ITokenStream tokens, IDiagnosticsCollector diagnostics)
            {
                return FakeParse<NameSyntax>(tokens, diagnostics);
            }
        }

        private class DeclarationParser : IDeclarationParser
        {
            public DeclarationSyntax ParseDeclaration([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
            {
                return FakeParse<DeclarationSyntax>(tokens, diagnostics);
            }
        }

        private class ParameterParser : IParameterParser
        {
            public ParameterSyntax ParseParameter([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
            {
                return FakeParse<ParameterSyntax>(tokens, diagnostics);
            }
        }

        private class BlockParser : IBlockParser
        {
            public BlockExpressionSyntax ParseBlock([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
            {
                return FakeParse<BlockExpressionSyntax>(tokens, diagnostics);
            }
        }

        private class UsingDirectiveParser : IUsingDirectiveParser
        {
            [NotNull]
            public UsingDirectiveSyntax ParseUsingDirective(
                [NotNull] ITokenStream tokens,
                IDiagnosticsCollector diagnostics)
            {
                var _ = tokens.Take<UsingKeywordToken>();

                var fakeToken = tokens.ExpectFake();
                return (UsingDirectiveSyntax)fakeToken?.FakeValue ?? throw new InvalidOperationException();
            }
        }

        private class ModifierParser : IModifierParser
        {
            public ModifierSyntax AcceptModifier(ITokenStream tokens, IDiagnosticsCollector diagnostics)
            {
                if (tokens.Current is FakeToken fake && fake.FakeValue is ModifierSyntax syntax)
                {
                    tokens.MoveNext();
                    return syntax;
                }

                return null;
            }
        }
    }
}
