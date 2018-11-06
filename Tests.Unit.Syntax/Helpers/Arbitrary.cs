using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using Fare;
using FsCheck;
using JetBrains.Annotations;
using LetKeywordToken = Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.LetKeywordToken;
using VarKeywordToken = Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.VarKeywordToken;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Helpers
{
    public class Arbitrary
    {
        public static Arbitrary<PsuedoToken> PsuedoToken()
        {
            return Arb.From(GenPsuedoToken());
        }

        public static Arbitrary<List<PsuedoToken>> PsuedoTokenList()
        {
            return Arb.From(GenPsuedoTokenList(), Arb.Shrink);
        }

        private static Gen<List<PsuedoToken>> GenPsuedoTokenList()
        {
            return Gen.Sized(size => GenPsuedoTokenList(size, size));
        }

        [NotNull]
        private static Gen<List<PsuedoToken>> GenPsuedoTokenList(int size, int length)
        {
            Requires.Positive(nameof(size), size);
            Requires.Positive(nameof(length), length);
            if (length == 0)
                return Gen.Fresh(() => new List<PsuedoToken>());

            return GenPsuedoTokenList(size, length - 1).Select(list => AppendPsuedoToken(size, list));
        }

        [NotNull]
        private static List<PsuedoToken> AppendPsuedoToken(
            int size,
            [NotNull] List<PsuedoToken> tokens)
        {
            var lastToken = tokens.LastOrDefault();
            // TODO this is a huge hack calling Sample() FIX IT!
            var token = GenPsuedoToken().Where(t =>
            {
                if (lastToken == null) return true;

                return !SeparateTokens(lastToken, t);

            }).Sample(size, 1).Single();
            tokens.Add(token);
            return tokens;
        }

        private static bool SeparateTokens([NotNull] PsuedoToken t1, [NotNull] PsuedoToken t2)
        {
            switch (t1.Text)
            {
                case ".":
                case "^":
                    return t2.Text == "." || t2.Text == ".." || t2.Text == "..<";
                case "+":
                case "*":
                case ">":
                    return t2.Text == "=" || t2.Text == "==" || t2.Text == "=/=" || t2.Text == "=>";
                case "<":
                    return t2.Text == "=" || t2.Text == "==" || t2.Text == "=/=" || t2.Text == "=>" || t2.Text == ":"
                        || t2.Text == ".." || t2.Text == "..<";
                case "-":
                    return t2.Text == "=" || t2.Text == "==" || t2.Text == "=/=" || t2.Text == "=>"
                        || t2.Text == ">" || t2.Text == ">=";
                case "/":
                    return t2.Text == "=" || t2.Text == "==" || t2.Text == "=/=" || t2.Text == "=>"
                        || t2.Text == "*" || t2.Text == "*="
                        || t2.Text == "/" || t2.Text == "/="
                        || t2.TokenType == typeof(CommentToken);
                case "=":
                    return t2.Text == "=" || t2.Text == "==" || t2.Text == "=/=" || t2.Text == "=>" || t2.Text == "/="
                        || t2.Text == ">" || t2.Text == ">=";
                case "$":
                    return t2.Text == ">" || t2.Text == ">="
                        || t2.Text == "<" || t2.Text == "<=" || t2.Text == "<:" || t2.Text == "<.." || t2.Text == "<..<";
                case "$>":
                case "$<":
                    return t2.Text == "≠" || t2.Text == "/=";
                case "?":
                    return t2.Text == "." || t2.Text == ".." || t2.Text == "..<"
                        || t2.Text == "?" || t2.Text == "?." || t2.Text == "??";
                case "..":
                case "<..":
                    return t2.Text == "<" || t2.Text == "<=" || t2.Text == "<:" || t2.Text == "<.." || t2.Text == "<..<";
                case "#":
                    return t2.Text == "#" || t2.Text == "##";
                default:
                    if (typeof(KeywordToken).IsAssignableFrom(t1.TokenType)
                        || typeof(IdentifierToken).IsAssignableFrom(t1.TokenType)
                        || typeof(KeywordOperatorToken).IsAssignableFrom(t1.TokenType)
                        )
                        return typeof(IdentifierToken).IsAssignableFrom(t2.TokenType)
                            || typeof(KeywordToken).IsAssignableFrom(t2.TokenType)
                            || typeof(KeywordOperatorToken).IsAssignableFrom(t2.TokenType)
                            || t2.TokenType == typeof(IntegerLiteralToken);
                    else if (t1.TokenType == typeof(IntegerLiteralToken))
                        return t2.TokenType == typeof(IntegerLiteralToken);
                    else if (t1.TokenType == typeof(WhitespaceToken))
                        return t2.TokenType == typeof(WhitespaceToken);
                    else
                        return false;
            }
        }

        [NotNull]
        private static Gen<PsuedoToken> GenPsuedoToken()
        {
            return Gen.Frequency(
                GenSymbol().WithWeight(20),
                GenWhitespace().WithWeight(10),
                GenComment().WithWeight(5),
                GenBareIdentifier().WithWeight(10),
                GenEscapedIdentifier().WithWeight(5),
                GenIntegerLiteral().WithWeight(5),
                GenStringLiteral().WithWeight(5))
                .AssertNotNull();
        }

        [NotNull]
        private static Gen<PsuedoToken> GenSymbol()
        {
            return Gen.Elements(Symbols.AsEnumerable())
                .Select(item => new PsuedoToken(item.Value, item.Key));
        }

        [NotNull]
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

        [NotNull]
        private static Gen<PsuedoToken> GenWhitespace()
        {
            return GenRegex("[ \t\n\r]")
                .Select(s => new PsuedoToken(typeof(WhitespaceToken), s));
        }

        [NotNull]
        private static Gen<PsuedoToken> GenComment()
        {
            // Covers both block comments and line comments
            // For line comments, end in newline requires escape sequences
            return GenRegex(@"(/\*(\**[^/])*\*/)|" + "(//.*[\r\n])")
                .Select(s => new PsuedoToken(typeof(CommentToken), s));
        }

        [NotNull]
        private static Gen<PsuedoToken> GenBareIdentifier()
        {
            return GenRegex(@"[a-zA-Z_][a-zA-Z_0-9]*")
                .Where(s => !Symbols.ContainsKey(s)) // don't emit keywords
                .Select(s => new PsuedoToken(typeof(BareIdentifierToken), s, s));
        }

        [NotNull]
        private static Gen<PsuedoToken> GenEscapedIdentifier()
        {
            return GenRegex(@"\\[a-zA-Z_0-9]+")
                .Where(s => !Symbols.ContainsKey(s)) // don't emit keywords
                .Select(s => new PsuedoToken(typeof(EscapedIdentifierToken), s, s.Substring(1)));
        }

        [NotNull]
        private static Gen<PsuedoToken> GenIntegerLiteral()
        {
            return GenRegex(@"0|[1-9][0-9]*")
                .Select(s => new PsuedoToken(typeof(IntegerLiteralToken), s, BigInteger.Parse(s)));
        }

        [NotNull]
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
            { "<..", typeof(LessThanDotDotToken) },
            { "..<", typeof(DotDotLessThanToken) },
            { "<..<", typeof(LessThanDotDotLessThanToken) },
            { ":", typeof(ColonToken) },
            { "<:", typeof(LessThanColonToken) },
            { "?", typeof(QuestionToken) },
            { "?.", typeof(QuestionDotToken) },
            { "??", typeof(QuestionQuestionToken) },
            { "|", typeof(PipeToken) },
            { "→", typeof(RightArrowToken) },
            { "->", typeof(RightArrowToken) },
            { "@", typeof(AtSignToken) },
            { "^", typeof(CaretToken) },
            { "^.", typeof(CaretDotToken) },
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
            { "$<", typeof(DollarLessThanToken) },
            { "$<≠", typeof(DollarLessThanNotEqualToken) },
            { "$</=", typeof(DollarLessThanNotEqualToken) },
            { "$>", typeof(DollarGreaterThanToken) },
            { "$>≠", typeof(DollarGreaterThanNotEqualToken) },
            { "$>/=", typeof(DollarGreaterThanNotEqualToken) },
            { "=>", typeof(EqualsGreaterThanToken) },
            { "#", typeof(HashToken) },
            { "##", typeof(HashHashToken) },
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
            { "not", typeof(NotKeywordToken) },
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
            { "self", typeof(SelfKeywordToken) },
            { "extend", typeof(ExtendKeywordToken) },
            { "type", typeof(TypeKeywordToken) },
            { "metatype", typeof(MetatypeKeywordToken) },
            { "true", typeof(TrueKeywordToken) },
            { "false", typeof(FalseKeywordToken) },
            { "mut", typeof(MutableKeywordToken) },
            { "params", typeof(ParamsKeywordToken) },
            { "may", typeof(MayKeywordToken) },
            { "no", typeof(NoKeywordToken) },
            { "throw", typeof(ThrowKeywordToken) },
            { "ref", typeof(RefKeywordToken) },
            { "abstract", typeof(AbstractKeywordToken) },
            { "get", typeof(GetKeywordToken) },
            { "set", typeof(SetKeywordToken) },
            { "requires", typeof(RequiresKeywordToken) },
            { "ensures", typeof(EnsuresKeywordToken) },
            { "invariant", typeof(InvariantKeywordToken) },
            { "where", typeof(WhereKeywordToken) },
            { "const", typeof(ConstKeywordToken) },
            { "alias", typeof(AliasKeywordToken) },
            { "uninitialized", typeof(UninitializedKeywordToken) },
            { "none", typeof(NoneKeywordToken) },
            { "operator", typeof(OperatorKeywordToken) },
            { "implicit", typeof(ImplicitKeywordToken) },
            { "explicit", typeof(ExplicitKeywordToken) },
            { "move", typeof(MoveKeywordToken) },
            { "copy", typeof(CopyKeywordToken) },
            { "match", typeof(MatchKeywordToken) },
            { "loop", typeof(LoopKeywordToken) },
            { "while", typeof(WhileKeywordToken) },
            { "break", typeof(BreakKeywordToken) },
            { "next", typeof(NextKeywordToken) },
            { "override", typeof(OverrideKeywordToken) },
            { "as", typeof(AsKeywordToken) },
            { "any", typeof(AnyKeywordToken) },
        }.AsReadOnly();
    }
}
