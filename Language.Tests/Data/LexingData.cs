using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Core.Syntax;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{

    public class LexingData
    {
        #region Singleton
        private static readonly Lazy<LexingData> instance = new Lazy<LexingData>(() => new LexingData());

        public static LexingData Instance => instance.Value;
        #endregion

        public readonly IReadOnlyList<TestToken> AllTokens;
        private readonly IReadOnlyList<TestToken> permuteTokens;
        private readonly IReadOnlyList<Tuple<TestTokenMatcher, TestTokenMatcher>> separateTokens;

        private readonly Lazy<IReadOnlyList<TestTokenSequence>> allTwoTokenSequences;
        public IReadOnlyList<TestTokenSequence> AllTwoTokenSequences => allTwoTokenSequences.Value;
        private readonly Lazy<IReadOnlyList<TestTokenSequence>> threeTokenSequences;
        public IReadOnlyList<TestTokenSequence> ThreeTokenSequences => threeTokenSequences.Value;
        private readonly Lazy<IReadOnlyList<TestTokenSequence>> fourTokenSequences;
        public IReadOnlyList<TestTokenSequence> FourTokenSequences => fourTokenSequences.Value;

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
            permuteTokens = AllTokens.Where(t => t.Permute).ToList().AsReadOnly();

            // Separate Tokens
            separateTokens = GetSeparateTokens(data);

            // Token Sequences
            allTwoTokenSequences = new Lazy<IReadOnlyList<TestTokenSequence>>(() =>
            {
                var allOneTokenSequences = AllTokens.Select(TestTokenSequence.Single).ToList().AsReadOnly();
                return GetSequencesWithOneMoreToken(allOneTokenSequences, AllTokens);
            });

            // For three and four token sequences, we only consider the permute tokens
            threeTokenSequences = new Lazy<IReadOnlyList<TestTokenSequence>>(() =>
            {
                var oneTokenSequences = permuteTokens.Select(TestTokenSequence.Single).ToList().AsReadOnly();
                var twoTokenSequences = GetSequencesWithOneMoreToken(oneTokenSequences);
                return GetSequencesWithOneMoreToken(twoTokenSequences);
            });
            fourTokenSequences = new Lazy<IReadOnlyList<TestTokenSequence>>(() => GetSequencesWithOneMoreToken(threeTokenSequences.Value));
        }

        private static JObject GetJsonLexingData()
        {
            var path = Path.Combine(LangTestsDirectory.Get(), "lex", "token_tests.json");
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

        private static TestToken ParseToken(JObject tokenJson)
        {
            var permute = tokenJson.Value<bool?>("permute") ?? true;
            var text = tokenJson.Value<string>("text");
            var kind = ParseKind(tokenJson["kind"], text);
            var isValid = tokenJson.Value<bool?>("is_valid") ?? true;
            var value = ParseValue(tokenJson["value"]);
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
                    return TestTokenKind.Keyword(Keywords.Map[text]);
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
                case JTokenType.Integer:
                    return value.ToObject<long>();
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
                .CrossJoin(crossWith ?? permuteTokens, (sequence, token) => sequence.Append(token))
                .Where(DoesNotEndInInvalidPair)
                .ToList()
                .AsReadOnly();
        }

        private bool DoesNotEndInInvalidPair(TestTokenSequence sequence)
        {
            var lastIndex = sequence.Tokens.Count - 1;
            var secondToLast = sequence.Tokens[lastIndex - 1];
            var last = sequence.Tokens[lastIndex];

            foreach (var matcherPair in separateTokens)
                if (matcherPair.Item1.Matches(secondToLast) && matcherPair.Item2.Matches(last))
                    return false;

            return true;
        }
    }
}
