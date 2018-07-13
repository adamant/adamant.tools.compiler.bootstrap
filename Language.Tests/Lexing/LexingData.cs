using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    using TestTokenMatchers = IDictionary<Tuple<TestTokenKind, TestTokenKind>, IList<Tuple<TestTokenMatcher, TestTokenMatcher>>>;

    public class LexingData
    {
        #region Singleton
        private static readonly Lazy<LexingData> instance = new Lazy<LexingData>(() => new LexingData());

        public static LexingData Instance { get { return instance.Value; } }
        #endregion

        public readonly IList<TestToken> AllTokens;
        private readonly TestTokenMatchers SeparateTokens;
        public readonly IList<TestTokenSequence> OneTokenSequences;
        public readonly IList<TestTokenSequence> TwoTokenSequences;
        public readonly IList<TestTokenSequence> ThreeTokenSequences;
        public readonly IList<TestTokenSequence> FourTokenSequences;

        public static TheoryData<TestTokenSequence> GetTheoryData(IEnumerable<TestTokenSequence> sequences)
        {
            var data = new TheoryData<TestTokenSequence>();
            foreach (var sequence in sequences)
            {
                data.Add(sequence);
            }
            return data;
        }

        private LexingData()
        {
            var data = GetJsonLexingData();

            // All Tokens
            var tokenData = (JArray)data["tokens"];
            var tokens = tokenData.Cast<JObject>().SelectMany(ParseToken);
            var whitespace = data["whitespace"].ToObject<string[]>().Select(text => TestToken.Whitespace(text)).ToList();
            var comments = data["comments"].ToObject<string[]>().Select(text => TestToken.Comment(text)).ToList();
            AllTokens = tokens.Concat(whitespace).Concat(comments).ToList().AsReadOnly();

            // Separate Tokens
            SeparateTokens = GetSeparateTokens(data);

            // Token Sequences
            OneTokenSequences = AllTokens.Select(TestTokenSequence.Single).ToList().AsReadOnly();
            TwoTokenSequences = GetSequencesWithOneMoreToken(OneTokenSequences);
            ThreeTokenSequences = GetSequencesWithOneMoreToken(TwoTokenSequences);
            FourTokenSequences = GetSequencesWithOneMoreToken(ThreeTokenSequences);
        }

        private static JObject GetJsonLexingData()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Lexing.json");
            if (!File.Exists(path))
                throw new ArgumentException($"Could not find file at path: {path}");
            var content = File.ReadAllText(path);
            var data = JObject.Parse(content);
            return data;
        }

        private static IEnumerable<TestToken> ParseToken(JObject token)
        {
            var kind = ParseKind(token["token"].ToObject<string>()).ToTokenKind();

            var valid = token["valid"];
            IEnumerable<TestToken> testTokens;
            switch (valid.Type)
            {
                case JTokenType.String:
                    testTokens = TestToken.Valid(kind, valid.ToObject<string>()).Yield();
                    break;
                case JTokenType.Array:
                    testTokens = valid.ToObject<JToken[]>().Select(validValue => ParseValidToken(kind, validValue));
                    break;
                default:
                    throw new NotSupportedException($"'valid' property does not support type {valid.Type}");
            }
            if (token.TryGetValue("invalid", out JToken invalid))
                testTokens = testTokens.Concat(invalid.ToObject<JToken[]>().Select(invalidValue => ParseInvalidToken(kind, invalidValue)));

            return testTokens;
        }

        private static TestTokenKind ParseKind(string kind)
        {
            switch (kind)
            {
                case "comment":
                    return TestTokenKind.Comment();
                case "whitespace":
                    return TestTokenKind.Whitespace();
                default:
                    return TestTokenKind.Token(Enum.Parse<TokenKind>(kind.Replace("_", ""), ignoreCase: true));
            }
        }

        private static TestToken ParseValidToken(TokenKind kind, JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.String:
                    return TestToken.Valid(kind, token.ToObject<string>());
                case JTokenType.Object:
                    return TestToken.Valid(kind, token["text"].ToObject<string>(), ParseValue(token["value"]));
                default:
                    throw new NotSupportedException($"'{token}' not supported as valid token");
            }
        }

        private static TestToken ParseInvalidToken(TokenKind kind, JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.String:
                    return TestToken.Invalid(kind, token.ToObject<string>());
                case JTokenType.Object:
                    return TestToken.Invalid(kind, token["text"].ToObject<string>(), ParseValue(token["value"]));
                default:
                    throw new NotSupportedException($"'{token}' not supported as valid token");
            }
        }

        private static object ParseValue(JToken value)
        {
            switch (value.Type)
            {
                case JTokenType.String:
                    return value.ToObject<string>();
                default:
                    throw new NotSupportedException($"'{value}' not supported as token value");
            }
        }

        private static TestTokenMatchers GetSeparateTokens(JObject data)
        {
            var separate = data["separate"].ToObject<JArray[]>();
            // Fill in every value so we don't have to worry about checking if they are there later
            var matchers = new Dictionary<Tuple<TestTokenKind, TestTokenKind>, IList<Tuple<TestTokenMatcher, TestTokenMatcher>>>();
            foreach (var sequence in separate)
            {
                if (sequence.Count != 2)
                    throw new InvalidDataException($"Separate sequence has more that two tokens `{sequence}`");

                var first = ParseTokenMatcher(sequence[0]);
                var second = ParseTokenMatcher(sequence[1]);
                var key = Tuple.Create(first.Kind, second.Kind);
                if (!matchers.ContainsKey(key))
                    matchers.Add(key, new List<Tuple<TestTokenMatcher, TestTokenMatcher>>());

                matchers[key].Add(Tuple.Create(first, second));
            }
            return matchers;
        }

        private static TestTokenMatcher ParseTokenMatcher(JToken matcher)
        {
            switch (matcher.Type)
            {
                case JTokenType.String:
                    return new TestTokenMatcher(ParseKind(matcher.ToObject<string>()), default(string));
                case JTokenType.Object:
                    var pair = ((JObject)matcher).Single<KeyValuePair<string, JToken>>();
                    return new TestTokenMatcher(ParseKind(pair.Key), pair.Value.ToObject<string>());
                default:
                    throw new NotSupportedException($"'{matcher}' not supported as token matcher");
            }
        }
        private IList<TestTokenSequence> GetSequencesWithOneMoreToken(IList<TestTokenSequence> sequences)
        {
            return sequences
                .SelectMany(_ => AllTokens, (sequence, token) => sequence.Append(token))
                .Where(DoesNotContainInvalidPair)
                .ToList()
                .AsReadOnly();
        }

        private bool DoesNotContainInvalidPair(TestTokenSequence sequence)
        {
            var separate = SeparateTokens;
            TestToken previous = null;
            foreach (var token in sequence.Tokens)
            {
                if (previous != null)
                {
                    var key = Tuple.Create(previous.Kind, token.Kind);
                    if (separate.TryGetValue(key, out var matchers))
                        foreach (var matcherPair in matchers)
                            if (matcherPair.Item1.Matches(previous) && matcherPair.Item2.Matches(token))
                                return false;
                }

                previous = token;
            }

            return true;
        }
    }
}
