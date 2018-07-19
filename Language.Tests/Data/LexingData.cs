using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
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

            AllTokens = ParseTokens(data.Value<JArray>("tokens"));
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

        private TestToken ParseToken(JObject tokenJson)
        {
            var permute = tokenJson.Value<bool?>("permute") ?? true;
            var text = tokenJson.Value<string>("text");
            var kind = ParseKind(tokenJson["kind"], text);
            var isValid = tokenJson.Value<bool?>("is_valid") ?? true;
            object value = ParseValue(tokenJson["value"]);
            return new TestToken(permute, kind, text, isValid, value);
        }

        private static TestTokenKind ParseKind(JToken kindJson, string text)
        {
            return ParseKind(kindJson.ToObject<string>(), text);
        }

        private static TestTokenKind ParseKind(string kind, string text)
        {
            switch (kind)
            {
                case "keyword":
                    if (text == null)
                        return TestTokenKind.Keyword();
                    return TestTokenKind.Keyword(Enum.Parse<TokenKind>(text + "Keyword", ignoreCase: true));
                case "comment":
                    return TestTokenKind.Comment();
                case "whitespace":
                    return TestTokenKind.Whitespace();
                default:
                    return TestTokenKind.Token(Enum.Parse<TokenKind>(kind.Replace("_", ""), ignoreCase: true));
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
                // The open matchers may refer to "keyword" that needs to be converted to all keywords
                var openMatcherPair = Tuple.Create(first, second);
                foreach (var matcherPair in ExpandKeywords(openMatcherPair))
                {
                    var key = Tuple.Create(matcherPair.Item1.Kind, matcherPair.Item2.Kind);
                    if (!matchers.ContainsKey(key))
                        matchers.Add(key, new List<Tuple<TestTokenMatcher, TestTokenMatcher>>());

                    matchers[key].Add(matcherPair);
                }
            }
            return matchers;
        }

        private static IEnumerable<Tuple<TestTokenMatcher, TestTokenMatcher>> ExpandKeywords(Tuple<TestTokenMatcher, TestTokenMatcher> matchers)
        {
            if (MatchesAnyKeyword(matchers.Item1.Kind))
            {
                if (MatchesAnyKeyword(matchers.Item2.Kind))
                    return Keywords().CrossJoin(Keywords());

                return Keywords().Select(k => Tuple.Create(k, matchers.Item2));
            }
            else if (MatchesAnyKeyword(matchers.Item2.Kind))
            {
                return Keywords().Select(k => Tuple.Create(matchers.Item1, k));
            }
            else
                return matchers.Yield();
        }

        private static bool MatchesAnyKeyword(TestTokenKind kind)
        {
            return kind.Category == TestTokenCategory.Keyword
                && kind.TokenKind == null;
        }

        private static IEnumerable<TestTokenMatcher> Keywords()
        {
            return Lexer.Keywords.Select(pair => new TestTokenMatcher(TestTokenKind.Keyword(pair.Value), pair.Key));
        }

        private static TestTokenMatcher ParseTokenMatcher(JToken matcher)
        {
            switch (matcher.Type)
            {
                case JTokenType.String:
                    return new TestTokenMatcher(ParseKind(matcher, null), default(string));
                case JTokenType.Object:
                    var pair = ((JObject)matcher).Single<KeyValuePair<string, JToken>>();
                    var value = pair.Value.ToObject<string>();
                    return new TestTokenMatcher(ParseKind(pair.Key, value), value);
                default:
                    throw new NotSupportedException($"'{matcher}' not supported as token matcher");
            }
        }
        private IList<TestTokenSequence> GetSequencesWithOneMoreToken(IList<TestTokenSequence> sequences)
        {
            return sequences
                .CrossJoin(PermuteTokens, (sequence, token) => sequence.Append(token))
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
