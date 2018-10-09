using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public static partial class TokenTypes
    {
        [NotNull]
        public static readonly IReadOnlyDictionary<string, Func<TextSpan, KeywordToken>> KeywordFactories;

        [NotNull]
        [ItemNotNull]
        public static readonly IReadOnlyList<Type> BooleanOperator = new List<Type>()
        {
            typeof(NotKeywordToken),
            typeof(AndKeywordToken),
            typeof(OrKeywordToken),
            typeof(XorKeywordToken),
        }.AsReadOnly();

        [NotNull]
        public static IReadOnlyDictionary<string, Func<TextSpan, BooleanOperatorToken>> BooleanOperatorFactories;

        private static readonly int KeywordTokenLength = "KeywordToken".Length;

        static TokenTypes()
        {
            KeywordFactories = BuildKeywordFactories();
            BooleanOperatorFactories = BuildBooleanOperatorFactories();
        }

        [NotNull]
        private static IReadOnlyDictionary<string, Func<TextSpan, KeywordToken>> BuildKeywordFactories()
        {
            var keywordFactories = new Dictionary<string, Func<TextSpan, KeywordToken>>();

            foreach (var tokenType in Keyword)
            {
                string keyword;
                var tokenTypeName = tokenType.Name;
                switch (tokenTypeName)
                {
                    // Some exceptions to the normal rule
                    case "FunctionKeywordToken":
                        keyword = "fn";
                        break;
                    case "SelfTypeKeywordToken":
                        keyword = "Self";
                        break;
                    default:
                        keyword = tokenTypeName
                            .Substring(0, tokenTypeName.Length - KeywordTokenLength)
                            .ToLower();
                        break;
                }
                var factory = CompileFactory<KeywordToken>(tokenType);
                keywordFactories.Add(keyword, factory);
            }
            return keywordFactories.AsReadOnly();
        }

        [NotNull]
        private static IReadOnlyDictionary<string, Func<TextSpan, BooleanOperatorToken>> BuildBooleanOperatorFactories()
        {
            var operatorFactories = new Dictionary<string, Func<TextSpan, BooleanOperatorToken>>();

            foreach (var tokenType in BooleanOperator)
            {
                var tokenTypeName = tokenType.Name;
                var keyword = tokenTypeName
                    .Substring(0, tokenTypeName.Length - KeywordTokenLength)
                    .ToLower();

                var factory = CompileFactory<BooleanOperatorToken>(tokenType);
                operatorFactories.Add(keyword, factory);
            }

            var booleanOperatorFactories = operatorFactories.AsReadOnly();
            return booleanOperatorFactories;
        }

        private static Func<TextSpan, T> CompileFactory<T>([NotNull] Type tokenType)
            where T : Token
        {
            var spanParam = Expression.Parameter(typeof(TextSpan), "span");
            var newExpression = Expression.New(tokenType.GetConstructor(new[] { typeof(TextSpan) }), spanParam);
            var factory =
                Expression.Lambda<Func<TextSpan, T>>(
                    newExpression, spanParam);
            return factory.Compile();
        }
    }
}
