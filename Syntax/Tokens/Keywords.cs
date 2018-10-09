using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public static partial class Keywords
    {
        [NotNull]
        public static IReadOnlyDictionary<string, Func<TextSpan, KeywordToken>> Factories;

        static Keywords()
        {
            var keywordMap = new Dictionary<string, Func<TextSpan, KeywordToken>>();

            var keywordTokenLength = "KeywordToken".Length;
            foreach (var tokenType in TokenTypes)
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
                            .Substring(0, tokenTypeName.Length - keywordTokenLength)
                            .ToLower();
                        break;
                }
                var spanParam = Expression.Parameter(typeof(TextSpan), "span");
                var newExpression = Expression.New(tokenType.GetConstructor(new[] { typeof(TextSpan) }), spanParam);
                var factory =
                    Expression.Lambda<Func<TextSpan, KeywordToken>>(
                        newExpression,
                        new ParameterExpression[] { spanParam });
                keywordMap.Add(keyword, factory.Compile());
            }
            Factories = keywordMap.AsReadOnly();
        }
    }
}
