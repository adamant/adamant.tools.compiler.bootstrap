using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public static partial class TokenTypes
    {
        [NotNull]
        public static readonly FixedDictionary<string, Func<TextSpan, IKeywordToken>> KeywordFactories;

        private static readonly int KeywordTokenLength = "KeywordToken".Length;

        static TokenTypes()
        {
            KeywordFactories = BuildKeywordFactories();
        }

        [NotNull]
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
                        keyword = tokenTypeName
                            .Substring(0, tokenTypeName.Length - KeywordTokenLength)
                            .ToLower();
                        break;
                }
                var factory = CompileFactory<IKeywordToken>(tokenType);
                factories.Add(keyword, factory);
            }
            return factories.ToFixedDictionary();
        }

        private static Func<TextSpan, T> CompileFactory<T>([NotNull] Type tokenType)
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
