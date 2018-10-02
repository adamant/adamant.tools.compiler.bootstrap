using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public static class Keywords
    {
        public static IReadOnlyList<TokenKind> TokenKinds;
        public static IReadOnlyDictionary<string, TokenKind> Map;

        static Keywords()
        {
            TokenKinds = Enum.GetValues(typeof(TokenKind))
                .Cast<TokenKind>()
                .Where(t => t.ToString().EndsWith("Keyword"))
                .ToList().AsReadOnly();

            var keywordDictionary = new Dictionary<string, TokenKind>();

            var keywordLength = "Keyword".Length;
            foreach (var tokenKind in TokenKinds)
            {
                string keyword;
                switch (tokenKind)
                {
                    // Some exceptions to the normal rule
                    case TokenKind.FunctionKeyword:
                        keyword = "fn";
                        break;
                    case TokenKind.SelfTypeKeyword:
                        keyword = "Self";
                        break;

                    default:
                        var enumName = tokenKind.ToString();
                        keyword = enumName
                            .Substring(0, enumName.Length - keywordLength)
                            .ToLower();
                        break;
                }
                keywordDictionary.Add(keyword, tokenKind);
            }

            Map = new ReadOnlyDictionary<string, TokenKind>(keywordDictionary);
        }
    }
}
