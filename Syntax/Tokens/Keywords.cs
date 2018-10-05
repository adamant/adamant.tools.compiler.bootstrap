using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public static class Keywords
    {
        public static IReadOnlyList<TokenKind> TokenKinds;
        public static IReadOnlyDictionary<string, TokenKind> Map;

        static Keywords()
        {
            // Some values have multiple names, to be sure we get the right things, we need to go by names not values
            var keywordNames = Enum.GetNames(typeof(TokenKind))
                .Where(n => n.EndsWith("Keyword"))
                .ToList()
                ;
            TokenKinds = keywordNames
                .Select(Enum.Parse<TokenKind>)
                .ToList().AsReadOnly();

            var keywordDictionary = new Dictionary<string, TokenKind>();

            var keywordLength = "Keyword".Length;
            foreach (var keywordName in keywordNames)
            {
                var tokenKind = Enum.Parse<TokenKind>(keywordName);
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
                        keyword = keywordName
                            .Substring(0, keywordName.Length - keywordLength)
                            .ToLower();
                        break;
                }
                keywordDictionary.Add(keyword, tokenKind);
            }

            Map = new ReadOnlyDictionary<string, TokenKind>(keywordDictionary);
        }
    }
}
