using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{

    public class LexingData
    {
        #region Singleton
        private static readonly Lazy<LexingData> instance = new Lazy<LexingData>(() => new LexingData());

        public static LexingData Instance { get { return instance.Value; } }
        #endregion

        public readonly IReadOnlyList<TestToken> AllTokens;
        private readonly IReadOnlyList<TestToken> PermuteTokens;
        private readonly IReadOnlyList<Tuple<TestTokenMatcher, TestTokenMatcher>> SeparateTokens;
        public readonly IReadOnlyList<TestTokenSequence> AllTwoTokenSequences;
        public readonly IReadOnlyList<TestTokenSequence> ThreeTokenSequences;
        public readonly IReadOnlyList<TestTokenSequence> FourTokenSequences;

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
            var allOneTokenSequences = AllTokens.Select(TestTokenSequence.Single).ToList().AsReadOnly();
            AllTwoTokenSequences = GetSequencesWithOneMoreToken(allOneTokenSequences, AllTokens);

            // For three and four token sequeneces, we only consider the permute tokens
            var oneTokenSequences = PermuteTokens.Select(TestTokenSequence.Single).ToList().AsReadOnly();
            var twoTokenSequences = GetSequencesWithOneMoreToken(oneTokenSequences);
            ThreeTokenSequences = GetSequencesWithOneMoreToken(twoTokenSequences);
            FourTokenSequences = GetSequencesWithOneMoreToken(ThreeTokenSequences);
        }

        private static JObject GetJsonLexingData()
        {
            var path = Path.Combine(TestsDirectory.Get(), "lex", "token_tests.json");
            if (!File.Exists(path))
                throw new ArgumentException($"Could not find file at path: {path}");
            var content = File.ReadAllText(path);
            var data = JObject.Parse(content);
            return data;
        }

        private IReadOnlyList<TestToken> ParseTokens(JArray tokensJson)
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
                case "token":
                    return TestTokenKind.AnyToken();
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

        private static IReadOnlyList<Tuple<TestTokenMatcher, TestTokenMatcher>> GetSeparateTokens(JObject data)
        {
            var separate = data["separate"].ToObject<JArray[]>();
            var matchers = new List<Tuple<TestTokenMatcher, TestTokenMatcher>>();
            foreach (var sequence in separate)
            {
                if (sequence.Count != 2)
                    throw new InvalidDataException($"Separate sequence has more that two tokens `{sequence}`");

                matchers.Add(Tuple.Create(ParseTokenMatcher(sequence[0]), ParseTokenMatcher(sequence[1])));
            }
            return matchers.AsReadOnly();
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
        private IReadOnlyList<TestTokenSequence> GetSequencesWithOneMoreToken(IReadOnlyList<TestTokenSequence> sequences, IReadOnlyList<TestToken> crossWith = null)
        {
            return sequences
                .CrossJoin(crossWith ?? PermuteTokens, (sequence, token) => sequence.Append(token))
                .Where(DoesNotContainEndInInvalidPair)
                .ToList()
                .AsReadOnly();
        }

        private bool DoesNotContainEndInInvalidPair(TestTokenSequence sequence)
        {
            var lastIndex = sequence.Tokens.Count - 1;
            var secondToLast = sequence.Tokens[lastIndex - 1];
            var last = sequence.Tokens[lastIndex];

            foreach (var matcherPair in SeparateTokens)
                if (matcherPair.Item1.Matches(secondToLast) && matcherPair.Item2.Matches(last))
                    return false;

            return true;
        }
    }
}
