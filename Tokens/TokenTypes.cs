using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public static partial class TokenTypes
    {
        // Must be before KeywordFactories because it is used in the construction of it
        private static readonly int KeywordTokenLength = "KeywordToken".Length;

        public static readonly FixedDictionary<string, Func<TextSpan, IKeywordToken>> KeywordFactories =
            BuildKeywordFactories();

        private static FixedDictionary<string, Func<TextSpan, IKeywordToken>> BuildKeywordFactories()
        {
            var factories = new Dictionary<string, Func<TextSpan, IKeywordToken>>();

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
                    case "IsolatedKeywordToken":
                        keyword = "iso";
                        break;
                    case "MutableKeywordToken":
                        keyword = "mut";
                        break;
                    case "AnyKeywordToken":
                        keyword = "Any";
                        break;
                    case "TypeKeywordToken":
                        keyword = "Type";
                        break;
                    case "UnderscoreKeywordToken":
                        keyword = "_";
                        break;
                    default:
#pragma warning disable CA1308 // Normalize strings to uppercase. Reason: this is not a normalization
                        keyword = tokenTypeName
                            .Substring(0, tokenTypeName.Length - KeywordTokenLength)
                            .ToLower(CultureInfo.InvariantCulture);
#pragma warning restore CA1308 // Normalize strings to uppercase
                        break;
                }
                var factory = CompileFactory<IKeywordToken>(tokenType);
                factories.Add(keyword, factory);
            }
            return factories.ToFixedDictionary();
        }

        private static Func<TextSpan, T> CompileFactory<T>(Type tokenType)
            where T : IToken
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
