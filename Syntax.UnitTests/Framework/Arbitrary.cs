using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Fare;
using FsCheck;

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
                .Select(s => new PsuedoToken(TokenKind.Whitespace, s));
        }

        private static Gen<PsuedoToken> GenComment()
        {
            // Covers both block comments and line comments
            return GenRegex(@"/\*(\**[^/])*\*/|//.*")
                .Select(s => new PsuedoToken(TokenKind.Comment, s));
        }

        private static Gen<PsuedoToken> GenIdentifier()
        {
            return GenRegex(@"[a-zA-Z_][a-zA-Z_0-9]*")
                .Where(s => !Symbols.ContainsKey(s)) // don't emit keywords
                .Select(s => new PsuedoToken(TokenKind.Identifier, s, s));
        }

        private static Gen<PsuedoToken> GenIntegerLiteral()
        {
            return GenRegex(@"0|[1-9][0-9]*")
                .Select(s => new PsuedoToken(TokenKind.IntegerLiteral, s, BigInteger.Parse(s)));
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

                    return new PsuedoToken(TokenKind.StringLiteral, s, value);
                });
        }

        public static IReadOnlyDictionary<string, TokenKind> Symbols = new Dictionary<string, TokenKind>()
        {
            { "{", TokenKind.OpenBrace },
            { "}", TokenKind.CloseBrace },
            { "(", TokenKind.OpenParen },
            { ")", TokenKind.CloseParen },
            { "[", TokenKind.OpenBracket },
            { "]", TokenKind.CloseBracket },
            { ";", TokenKind.Semicolon },
            { ",", TokenKind.Comma },
            { ".", TokenKind.Dot },
            { "..", TokenKind.DotDot },
            { ":", TokenKind.Colon },
            { "?", TokenKind.Question },
            { "|", TokenKind.Pipe },
            { "→", TokenKind.RightArrow },
            { "->", TokenKind.RightArrow },
            { "@", TokenKind.AtSign },
            { "^", TokenKind.Caret },
            { "+", TokenKind.Plus },
            { "-", TokenKind.Minus },
            { "*", TokenKind.Asterisk },
            { "/", TokenKind.Slash },
            { "=", TokenKind.Equals },
            { "==", TokenKind.EqualsEquals },
            { "≠", TokenKind.NotEqual },
            { "=/=", TokenKind.NotEqual },
            { ">", TokenKind.GreaterThan },
            { "≥", TokenKind.GreaterThanOrEqual },
            { "⩾", TokenKind.GreaterThanOrEqual },
            { ">=", TokenKind.GreaterThanOrEqual },
            { "<", TokenKind.LessThan },
            { "≤", TokenKind.LessThanOrEqual },
            { "⩽", TokenKind.LessThanOrEqual },
            { "<=", TokenKind.LessThanOrEqual },
            { "+=", TokenKind.PlusEquals },
            { "-=", TokenKind.MinusEquals },
            { "*=", TokenKind.AsteriskEquals },
            { "/=", TokenKind.SlashEquals },
            { "$", TokenKind.Dollar },
            { "public", TokenKind.PublicKeyword },
            { "private", TokenKind.PrivateKeyword },
            { "let", TokenKind.LetKeyword },
            { "var", TokenKind.VarKeyword },
            { "void", TokenKind.VoidKeyword },
            { "int", TokenKind.IntKeyword },
            { "uint", TokenKind.UIntKeyword },
            { "bool", TokenKind.BoolKeyword },
            { "string", TokenKind.StringKeyword },
            { "return", TokenKind.ReturnKeyword },
            { "class", TokenKind.ClassKeyword },
            { "new", TokenKind.NewKeyword },
            { "delete", TokenKind.DeleteKeyword },
            { "namespace", TokenKind.NamespaceKeyword },
            { "using", TokenKind.UsingKeyword },
            { "foreach", TokenKind.ForeachKeyword },
            { "in", TokenKind.InKeyword },
            { "if", TokenKind.IfKeyword },
            { "else", TokenKind.ElseKeyword },
            { "and", TokenKind.AndKeyword },
            { "or", TokenKind.OrKeyword },
            { "xor", TokenKind.XorKeyword },
            { "struct", TokenKind.StructKeyword },
            { "enum", TokenKind.EnumKeyword },
            { "byte", TokenKind.ByteKeyword },
            { "size", TokenKind.SizeKeyword },
            { "protected", TokenKind.ProtectedKeyword },
            { "unsafe", TokenKind.UnsafeKeyword },
            { "safe", TokenKind.SafeKeyword },
            { "base", TokenKind.BaseKeyword },
            { "fn", TokenKind.FunctionKeyword },
            { "Self", TokenKind.SelfTypeKeyword },
            { "init", TokenKind.InitKeyword },
            { "owned", TokenKind.OwnedKeyword },
            { "self", TokenKind.SelfKeyword }
        }.AsReadOnly();

        //public static IReadOnlyDictionary<TokenKind, List<Func<PsuedoToken, PsuedoToken, bool>>> PairRestrictions = new ReadOnlyDictionary<TokenKind, List<Func<PsuedoToken, PsuedoToken, bool>>>(
        //    new Dictionary<TokenKind, List<Func<PsuedoToken, PsuedoToken, bool>>>()
        //    {
        //        {TokenKind.Dot, }
        //    });
    }
}
