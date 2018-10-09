using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Fare;
using FsCheck;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Framework
{
    public class Arbitrary
    {
        public static Arbitrary<PsuedoToken> PsuedoToken()
        {
            return Arb.From(GenPsuedoToken());
        }

        public static Arbitrary<List<PsuedoToken>> PsuedoTokenList()
        {
            return Arb.From(GenPsuedoTokenList());
        }

        private static Gen<List<PsuedoToken>> GenPsuedoTokenList()
        {
            //Gen.ListOf(Gen.Elements("fizz", "buzz", "bazz", "+", " ", "if")).Where(list => ...);
            //Gen.Sized(size =>
            //{
            //    Arb.Generate<int>().Select(length =>
            //    {
            //        var tokens = new List<PsuedoToken>(length);
            //        for (int i = 0; i < length; i++)
            //        {
            //            var lastToken = tokens.LastOrDefault();
            //            var newToken = GenPsuedoToken().
            //        }

            //        return tokens;
            //    });
            //});
            //return Gen.ListOf(GenPsuedoToken()).Select(l => l.ToList());
            throw new NotImplementedException();
        }

        //private static IEnumerable<Gen<PsuedoToken>> GenPsuedoTokens()
        //{
        //    var lastToken = null;

        //}

        private static Gen<PsuedoToken> GenPsuedoToken()
        {
            return Gen.OneOf(
                GenSymbol(),
                GenWhitespace(),
                GenComment(),
                GenIdentifier(),
                GenIntegerLiteral(),
                GenStringLiteral());
        }

        private static Gen<PsuedoToken> GenSymbol()
        {
            return Gen.Elements(Symbols.AsEnumerable())
                .Select(item => new PsuedoToken(item.Value, item.Key));
        }

        private static Gen<string> GenRegex(string pattern)
        {
            return Gen.Sized(size =>
            {
                var xegar = new Xeger(pattern);
                var count = size < 1 ? 1 : size;
                return Gen.Elements(Enumerable.Range(1, count).Select(i => xegar.Generate()))
                    .Resize(count);
            });
        }

        private static Gen<PsuedoToken> GenWhitespace()
        {
            return GenRegex("[ \t\n\r]")
                .Select(s => new PsuedoToken(typeof(WhitespaceToken), s));
        }

        private static Gen<PsuedoToken> GenComment()
        {
            // Covers both block comments and line comments
            return GenRegex(@"/\*(\**[^/])*\*/|//.*")
                .Select(s => new PsuedoToken(typeof(CommentToken), s));
        }

        private static Gen<PsuedoToken> GenIdentifier()
        {
            return GenRegex(@"[a-zA-Z_][a-zA-Z_0-9]*")
                .Where(s => !Symbols.ContainsKey(s)) // don't emit keywords
                .Select(s => new PsuedoToken(typeof(IdentifierToken), s, s));
        }

        private static Gen<PsuedoToken> GenIntegerLiteral()
        {
            return GenRegex(@"0|[1-9][0-9]*")
                .Select(s => new PsuedoToken(typeof(IntegerLiteralToken), s, BigInteger.Parse(s)));
        }

        private static Gen<PsuedoToken> GenStringLiteral()
        {
            // @"""([^\\]|\\(r|n|0|t|'|""|\\|u\([0-9a-fA-F]{1,6}\)))*"""
            return GenRegex(@"\""([^\\""]|\\(r|n|0|t|\'|\""))*\""")
                .Select(s =>
                {
                    var value = s.Substring(1, s.Length - 2)
                        .Replace(@"\\", @"\b") // Swap out backslash escape to not mess up others
                        .Replace(@"\r", "\r")
                        .Replace(@"\n", "\n")
                        .Replace(@"\0", "\0")
                        .Replace(@"\t", "\t")
                        .Replace(@"\'", "\'")
                        .Replace(@"\""", "\"")
                        .Replace(@"\b", "\\");

                    return new PsuedoToken(typeof(StringLiteralToken), s, value);
                });
        }

        [NotNull]
        public static IReadOnlyDictionary<string, Type> Symbols = new Dictionary<string, Type>()
        {
            { "{", typeof(OpenBraceToken) },
            { "}", typeof(CloseBraceToken) },
            { "(", typeof(OpenParenToken) },
            { ")", typeof(CloseParenToken) },
            { "[", typeof(OpenBracketToken) },
            { "]", typeof(CloseBracketToken) },
            { ";", typeof(SemicolonToken) },
            { ",", typeof(CommaToken) },
            { ".", typeof(DotToken) },
            { "..", typeof(DotDotToken) },
            { ":", typeof(ColonToken) },
            { "?", typeof(QuestionToken) },
            { "|", typeof(PipeToken) },
            { "→", typeof(RightArrowToken) },
            { "->", typeof(RightArrowToken) },
            { "@", typeof(AtSignToken) },
            { "^", typeof(CaretToken) },
            { "+", typeof(PlusToken) },
            { "-", typeof(MinusToken) },
            { "*", typeof(AsteriskToken) },
            { "/", typeof(SlashToken) },
            { "=", typeof(EqualsToken) },
            { "==", typeof(EqualsEqualsToken) },
            { "≠", typeof(NotEqualToken) },
            { "=/=", typeof(NotEqualToken) },
            { ">", typeof(GreaterThanToken) },
            { "≥", typeof(GreaterThanOrEqualToken) },
            { "⩾", typeof(GreaterThanOrEqualToken) },
            { ">=", typeof(GreaterThanOrEqualToken) },
            { "<", typeof(LessThanToken) },
            { "≤", typeof(LessThanOrEqualToken) },
            { "⩽", typeof(LessThanOrEqualToken) },
            { "<=", typeof(LessThanOrEqualToken) },
            { "+=", typeof(PlusEqualsToken) },
            { "-=", typeof(MinusEqualsToken) },
            { "*=", typeof(AsteriskEqualsToken) },
            { "/=", typeof(SlashEqualsToken) },
            { "$", typeof(DollarToken) },
            { "public", typeof(PublicKeywordToken) },
            { "private", typeof(PrivateKeywordToken) },
            { "let", typeof(LetKeywordToken) },
            { "var", typeof(VarKeywordToken) },
            { "void", typeof(VoidKeywordToken) },
            { "int", typeof(IntKeywordToken) },
            { "uint", typeof(UIntKeywordToken) },
            { "bool", typeof(BoolKeywordToken) },
            { "string", typeof(StringKeywordToken) },
            { "return", typeof(ReturnKeywordToken) },
            { "class", typeof(ClassKeywordToken) },
            { "new", typeof(NewKeywordToken) },
            { "delete", typeof(DeleteKeywordToken) },
            { "namespace", typeof(NamespaceKeywordToken) },
            { "using", typeof(UsingKeywordToken) },
            { "foreach", typeof(ForeachKeywordToken) },
            { "in", typeof(InKeywordToken) },
            { "if", typeof(IfKeywordToken) },
            { "else", typeof(ElseKeywordToken) },
            { "and", typeof(AndKeywordToken) },
            { "or", typeof(OrKeywordToken) },
            { "xor", typeof(XorKeywordToken) },
            { "struct", typeof(StructKeywordToken) },
            { "enum", typeof(EnumKeywordToken) },
            { "byte", typeof(ByteKeywordToken) },
            { "size", typeof(SizeKeywordToken) },
            { "protected", typeof(ProtectedKeywordToken) },
            { "unsafe", typeof(UnsafeKeywordToken) },
            { "safe", typeof(SafeKeywordToken) },
            { "base", typeof(BaseKeywordToken) },
            { "fn", typeof(FunctionKeywordToken) },
            { "Self", typeof(SelfTypeKeywordToken) },
            { "init", typeof(InitKeywordToken) },
            { "owned", typeof(OwnedKeywordToken) },
            { "self", typeof(SelfKeywordToken) }
        }.AsReadOnly();

        //public static IReadOnlyDictionary<TokenKind, List<Func<PsuedoToken, PsuedoToken, bool>>> PairRestrictions = new ReadOnlyDictionary<TokenKind, List<Func<PsuedoToken, PsuedoToken, bool>>>(
        //    new Dictionary<TokenKind, List<Func<PsuedoToken, PsuedoToken, bool>>>()
        //    {
        //        {TokenKind.Dot, }
        //    });
    }
}
