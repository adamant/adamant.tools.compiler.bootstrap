using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
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
        private readonly IList<TestToken> PermuteTokens;
        private readonly TestTokenMatchers SeparateTokens;
        public readonly IList<TestTokenSequence> TwoTokenSequences;
        public readonly IList<TestTokenSequence> ThreeTokenSequences;
        public readonly IList<TestTokenSequence> FourTokenSequences;

        public static TheoryData<TestToken> GetTheoryData(IEnumerable<TestToken> tokens)
        {
            var data = new TheoryData<TestToken>();
            foreach (var token in tokens)
            {
                data.Add(token);
            }
            return data;
        }
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

            // Tests
            var tokenTests = ParseTokens(data.Value<JArray>("tokens"));

            // All Tokens
            var tokens = data.Value<JArray>("permute_tokens").Cast<JObject>().SelectMany(ParseTokenGroup);
            var whitespace = data["whitespace"].ToObject<string[]>().Select(text => TestToken.Whitespace(true, text)).ToList();
            var comments = data["comments"].ToObject<string[]>().Select(text => TestToken.Comment(true, text)).ToList();
            AllTokens = tokenTests.Concat(tokens).Concat(whitespace).Concat(comments).ToList().AsReadOnly();
            PermuteTokens = AllTokens.Where(t => t.Permute).ToList().AsReadOnly();

            // Separate Tokens
            SeparateTokens = GetSeparateTokens(data);

            // Token Sequences
            var oneTokenSequences = PermuteTokens.Select(TestTokenSequence.Single).ToList().AsReadOnly();
            TwoTokenSequences = GetSequencesWithOneMoreToken(oneTokenSequences);
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

        private IList<TestToken> ParseTokens(JArray tokensJson)
        {
            return tokensJson.Cast<JObject>().Select(ParseToken).ToList().AsReadOnly();
        }

        private IList<TestTokenSequence> ParseSequenceTests(JArray testsJson)
        {
            return testsJson.Cast<JArray>().Select(ParseSequenceTest).ToList().AsReadOnly();
        }

        private TestTokenSequence ParseSequenceTest(JArray testJson)
        {
            return new TestTokenSequence(testJson.Cast<JObject>().Select(ParseToken));
        }

        private TestToken ParseToken(JObject tokenJson)
        {
            var permute = tokenJson.Value<bool?>("permute") ?? true;
            var kind = ParseKind(tokenJson["kind"]);
            var text = tokenJson.Value<string>("text");
            var isValid = tokenJson.Value<bool?>("is_valid") ?? true;
            object value = ParseValue(tokenJson["value"]);
            return new TestToken(permute, kind, text, true, value);
        }

        private static IEnumerable<TestToken> ParseTokenGroup(JObject tokenGroupJson)
        {
            var kind = ParseKind(tokenGroupJson["kind"]).ToTokenKind();

            var validValuesJson = tokenGroupJson["valid"];
            IEnumerable<TestToken> testTokens;
            switch (validValuesJson.Type)
            {
                case JTokenType.String:
                    testTokens = ParseValidValue(kind, validValuesJson).Yield();
                    break;
                case JTokenType.Array:
                    testTokens = validValuesJson.ToObject<JToken[]>().Select(validValue => ParseValidValue(kind, validValue));
                    break;
                default:
                    throw new NotSupportedException($"'valid' property does not support type {validValuesJson.Type}");
            }
            if (tokenGroupJson.TryGetValue("invalid", out JToken invalidValuesJson))
                testTokens = testTokens.Concat(invalidValuesJson.ToObject<JToken[]>().Select(invalidValue => ParseInvalidValue(kind, invalidValue)));

            return testTokens;
        }

        private static TestTokenKind ParseKind(JToken kindJson)
        {
            return ParseKind(kindJson.ToObject<string>());
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

        private static TestToken ParseValidValue(TokenKind kind, JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.String:
                    return TestToken.Valid(true, kind, token.ToObject<string>());
                case JTokenType.Object:
                    return TestToken.Valid(true, kind, token["text"].ToObject<string>(), ParseValue(token["value"]));
                default:
                    throw new NotSupportedException($"'{token}' not supported as valid token");
            }
        }

        private static TestToken ParseInvalidValue(TokenKind kind, JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.String:
                    return TestToken.Invalid(true, kind, token.ToObject<string>());
                case JTokenType.Object:
                    return TestToken.Invalid(true, kind, token["text"].ToObject<string>(), ParseValue(token["value"]));
                default:
                    throw new NotSupportedException($"'{token}' not supported as valid token");
            }
        }

        private static object ParseValue(JToken value)
        {
            if (value == null) return null; // null when property not present
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
                .SelectMany(_ => PermuteTokens, (sequence, token) => sequence.Append(token))
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
