using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Fare;
using FsCheck;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Helpers
{
    public static class Arbitrary
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

        private static Gen<List<PsuedoToken>> GenPsuedoTokenList(int size, int length)
        {
            Requires.Positive(nameof(size), size);
            Requires.Positive(nameof(length), length);
            if (length == 0)
                return Gen.Fresh(() => new List<PsuedoToken>());

            return GenPsuedoTokenList(size, length - 1).Select(list => AppendPsuedoToken(size, list));
        }

        private static List<PsuedoToken> AppendPsuedoToken(
            int size,
            List<PsuedoToken> tokens)
        {
            var lastToken = tokens.LastOrDefault();
            // TODO this is a huge hack calling Sample() FIX IT!
            var token = GenPsuedoToken().Where(t =>
            {
                if (lastToken == null)
                    return true;

                return !SeparateTokens(lastToken, t);

            }).Sample(size, 1).Single();
            tokens.Add(token);
            return tokens;
        }

        private static bool SeparateTokens(PsuedoToken t1, PsuedoToken t2)
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
                    return t2.Text == "=" || t2.Text == "==" || t2.Text == "=/=" || t2.Text == "=>"
                        || t2.Text == ":" || t2.Text == "::."
                        || t2.Text == ".." || t2.Text == "..<";
                case "-":
                    return t2.Text == "=" || t2.Text == "==" || t2.Text == "=/=" || t2.Text == "=>"
                        || t2.Text == ">" || t2.Text == ">=";
                case "/":
                    return t2.Text == "=" || t2.Text == "==" || t2.Text == "=/=" || t2.Text == "=>"
                        || t2.Text == "*" || t2.Text == "*="
                        || t2.Text == "/" || t2.Text == "/="
                        || t2.TokenType == typeof(ICommentToken);
                case "=":
                    return t2.Text == "=" || t2.Text == "==" || t2.Text == "=/=" || t2.Text == "=>" || t2.Text == "/="
                        || t2.Text == ">" || t2.Text == ">=";
                case "?":
                    return t2.Text == "." || t2.Text == ".." || t2.Text == "..<"
                        || t2.Text == "?" || t2.Text == "?." || t2.Text == "??";
                case "..":
                case "<..":
                    return t2.Text == "<" || t2.Text == "<=" || t2.Text == "<:" || t2.Text == "<.." || t2.Text == "<..<";
                case "#":
                    return t2.Text == "#" || t2.Text == "##";
                case ":":
                    return t2.Text == ":" || t2.Text == "::."; // TODO this should really be blocking the sequence ':',':','.'
                default:
                    if (typeof(IKeywordToken).IsAssignableFrom(t1.TokenType)
                        || typeof(IIdentifierToken).IsAssignableFrom(t1.TokenType)
                        )
                        return typeof(IIdentifierToken).IsAssignableFrom(t2.TokenType)
                            || typeof(IKeywordToken).IsAssignableFrom(t2.TokenType)
                            || t2.TokenType == typeof(IIntegerLiteralToken);
                    else if (t1.TokenType == typeof(IIntegerLiteralToken))
                        return t2.TokenType == typeof(IIntegerLiteralToken);
                    else if (t1.TokenType == typeof(IWhitespaceToken))
                        return t2.TokenType == typeof(IWhitespaceToken);
                    else
                        return false;
            }
        }

        private static Gen<PsuedoToken> GenPsuedoToken()
        {
            return Gen.Frequency(
                GenSymbol().WithWeight(20),
                GenWhitespace().WithWeight(10),
                GenComment().WithWeight(5),
                GenBareIdentifier().WithWeight(10),
                GenEscapedIdentifier().WithWeight(5),
                GenIntegerLiteral().WithWeight(5),
                GenStringLiteral().WithWeight(5));
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
                .Select(s => new PsuedoToken(typeof(IWhitespaceToken), s));
        }

        private static Gen<PsuedoToken> GenComment()
        {
            // Covers both block comments and line comments
            // For line comments, end in newline requires escape sequences
            return GenRegex(@"(/\*(\**[^/])*\*/)|" + "(//.*[\r\n])")
                .Select(s => new PsuedoToken(typeof(ICommentToken), s));
        }

        private static Gen<PsuedoToken> GenBareIdentifier()
        {
            return GenRegex(@"[a-zA-Z_][a-zA-Z_0-9]*")
                .Where(s => !Symbols.ContainsKey(s)) // don't emit keywords
                .Select(s => new PsuedoToken(typeof(IBareIdentifierToken), s, s));
        }

        private static Gen<PsuedoToken> GenEscapedIdentifier()
        {
            return GenRegex(@"\\[a-zA-Z_0-9]+")
                .Where(s => !Symbols.ContainsKey(s)) // don't emit keywords
                .Select(s => new PsuedoToken(typeof(IEscapedIdentifierToken), s, s.Substring(1)));
        }

        private static Gen<PsuedoToken> GenIntegerLiteral()
        {
            return GenRegex(@"0|[1-9][0-9]*")
                .Select(s => new PsuedoToken(typeof(IIntegerLiteralToken), s, BigInteger.Parse(s, CultureInfo.InvariantCulture)));
        }

        private static Gen<PsuedoToken> GenStringLiteral()
        {
            // @"""([^\\]|\\(r|n|0|t|'|""|\\|u\([0-9a-fA-F]{1,6}\)))*"""
            return GenRegex(@"\""([^\\""]|\\(r|n|0|t|\'|\""))*\""")
                .Select(s =>
                {
                    var value = s[1..^1]
                        .Replace(@"\\", @"\b", StringComparison.Ordinal) // Swap out backslash escape to not mess up others
                        .Replace(@"\r", "\r", StringComparison.Ordinal)
                        .Replace(@"\n", "\n", StringComparison.Ordinal)
                        .Replace(@"\0", "\0", StringComparison.Ordinal)
                        .Replace(@"\t", "\t", StringComparison.Ordinal)
                        .Replace(@"\'", "\'", StringComparison.Ordinal)
                        .Replace(@"\""", "\"", StringComparison.Ordinal)
                        .Replace(@"\b", "\\", StringComparison.Ordinal);

                    return new PsuedoToken(typeof(IStringLiteralToken), s, value);
                });
        }

        public static readonly FixedDictionary<string, Type> Symbols = new Dictionary<string, Type>()
        {
            { "{", typeof(IOpenBraceToken) },
            { "}", typeof(ICloseBraceToken) },
            { "(", typeof(IOpenParenToken) },
            { ")", typeof(ICloseParenToken) },
            //{ "[", typeof(IOpenBracketToken) },
            //{ "]", typeof(ICloseBracketToken) },
            { ";", typeof(ISemicolonToken) },
            { ",", typeof(ICommaToken) },
            { ".", typeof(IDotToken) },
            { "::.", typeof(IColonColonDotToken) },
            { "..", typeof(IDotDotToken) },
            { "<..", typeof(ILessThanDotDotToken) },
            { "..<", typeof(IDotDotLessThanToken) },
            { "<..<", typeof(ILessThanDotDotLessThanToken) },
            { ":", typeof(IColonToken) },
            { "<:", typeof(ILessThanColonToken) },
            { "?", typeof(IQuestionToken) },
            { "?.", typeof(IQuestionDotToken) },
            { "??", typeof(IQuestionQuestionToken) },
            //{ "|", typeof(IPipeToken) },
            { "→", typeof(IRightArrowToken) },
            { "->", typeof(IRightArrowToken) },
            //{ "@", typeof(IAtSignToken) },
            //{ "^", typeof(ICaretToken) },
            //{ "^.", typeof(ICaretDotToken) },
            { "+", typeof(IPlusToken) },
            { "-", typeof(IMinusToken) },
            { "*", typeof(IAsteriskToken) },
            { "/", typeof(ISlashToken) },
            { "=", typeof(IEqualsToken) },
            { "==", typeof(IEqualsEqualsToken) },
            { "≠", typeof(INotEqualToken) },
            { "=/=", typeof(INotEqualToken) },
            { ">", typeof(IGreaterThanToken) },
            { "≥", typeof(IGreaterThanOrEqualToken) },
            { ">=", typeof(IGreaterThanOrEqualToken) },
            { "<", typeof(ILessThanToken) },
            { "≤", typeof(ILessThanOrEqualToken) },
            { "<=", typeof(ILessThanOrEqualToken) },
            { "+=", typeof(IPlusEqualsToken) },
            { "-=", typeof(IMinusEqualsToken) },
            { "*=", typeof(IAsteriskEqualsToken) },
            { "/=", typeof(ISlashEqualsToken) },
            { "$", typeof(IDollarToken) },
            { "=>", typeof(IEqualsGreaterThanToken) },
            //{ "#", typeof(IHashToken) },
            //{ "##", typeof(IHashHashToken) },
            { "published", typeof(IPublishedKeywordToken) },
            { "public", typeof(IPublicKeywordToken) },
            //{ "protected", typeof(IProtectedKeywordToken) },
            { "let", typeof(ILetKeywordToken) },
            { "var", typeof(IVarKeywordToken) },
            { "void", typeof(IVoidKeywordToken) },
            //{ "int8", typeof(IInt8KeywordToken) },
            //{ "int16", typeof(IInt16KeywordToken) },
            { "int", typeof(IIntKeywordToken) },
            //{ "int64", typeof(IInt64KeywordToken) },
            { "byte", typeof(IByteKeywordToken) },
            //{ "uint16", typeof(IUInt16KeywordToken) },
            { "uint", typeof(IUIntKeywordToken) },
            //{ "uint64", typeof(IUInt64KeywordToken) },
            { "bool", typeof(IBoolKeywordToken) },
            { "return", typeof(IReturnKeywordToken) },
            { "class", typeof(IClassKeywordToken) },
            { "new", typeof(INewKeywordToken) },
            //{ "delete", typeof(IDeleteKeywordToken) },
            { "namespace", typeof(INamespaceKeywordToken) },
            { "using", typeof(IUsingKeywordToken) },
            { "foreach", typeof(IForeachKeywordToken) },
            { "in", typeof(IInKeywordToken) },
            { "if", typeof(IIfKeywordToken) },
            { "else", typeof(IElseKeywordToken) },
            { "not", typeof(INotKeywordToken) },
            { "and", typeof(IAndKeywordToken) },
            { "or", typeof(IOrKeywordToken) },
            //{ "struct", typeof(IStructKeywordToken) },
            //{ "enum", typeof(IEnumKeywordToken) },
            { "size", typeof(ISizeKeywordToken) },
            { "unsafe", typeof(IUnsafeKeywordToken) },
            { "safe", typeof(ISafeKeywordToken) },
            //{ "base", typeof(IBaseKeywordToken) },
            { "fn", typeof(IFunctionKeywordToken) },
            //{ "Self", typeof(ISelfTypeKeywordToken) },
            //{ "init", typeof(IInitKeywordToken) },
            { "owned", typeof(IOwnedKeywordToken) },
            { "forever", typeof(IForeverKeywordToken) },
            { "self", typeof(ISelfKeywordToken) },
            //{ "Type", typeof(ITypeKeywordToken) },
            { "true", typeof(ITrueKeywordToken) },
            { "false", typeof(IFalseKeywordToken) },
            { "mut", typeof(IMutableKeywordToken) },
            //{ "params", typeof(IParamsKeywordToken) },
            //{ "may", typeof(IMayKeywordToken) },
            //{ "no", typeof(INoKeywordToken) },
            //{ "throw", typeof(IThrowKeywordToken) },
            //{ "ref", typeof(IRefKeywordToken) },
            //{ "abstract", typeof(IAbstractKeywordToken) },
            //{ "get", typeof(IGetKeywordToken) },
            //{ "set", typeof(ISetKeywordToken) },
            //{ "requires", typeof(IRequiresKeywordToken) },
            //{ "ensures", typeof(IEnsuresKeywordToken) },
            //{ "invariant", typeof(IInvariantKeywordToken) },
            //{ "where", typeof(IWhereKeywordToken) },
            //{ "const", typeof(IConstKeywordToken) },
            //{ "uninitialized", typeof(IUninitializedKeywordToken) },
            { "none", typeof(INoneKeywordToken) },
            //{ "operator", typeof(IOperatorKeywordToken) },
            //{ "implicit", typeof(IImplicitKeywordToken) },
            //{ "explicit", typeof(IExplicitKeywordToken) },
            { "move", typeof(IMoveKeywordToken) },
            { "copy", typeof(ICopyKeywordToken) },
            //{ "match", typeof(IMatchKeywordToken) },
            { "loop", typeof(ILoopKeywordToken) },
            { "while", typeof(IWhileKeywordToken) },
            { "break", typeof(IBreakKeywordToken) },
            { "next", typeof(INextKeywordToken) },
            //{ "override", typeof(IOverrideKeywordToken) },
            { "as", typeof(IAsKeywordToken) },
            { "Any", typeof(IAnyKeywordToken) },
            { "never", typeof(INeverKeywordToken) },
            //{ "float", typeof(IFloatKeywordToken) },
            //{ "float32", typeof(IFloat32KeywordToken) },
            { "offset", typeof(IOffsetKeywordToken) },
            //{ "_", typeof(IUnderscoreKeywordToken) },
            //{ "external", typeof(IExternalKeywordToken) },
        }.ToFixedDictionary();
    }
}
