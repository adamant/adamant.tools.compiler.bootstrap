using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class Lexing4Tokens
    {
        [Theory]
        [MemberData(nameof(GetFourTokenSequenceData))]
        internal void LexesAllValidTokenCombinations(string text, TestToken[] expectedTokens)
        {
            LexAssert.LexesTo(text, expectedTokens);
        }

        public static IEnumerable<object[]> GetFourTokenSequenceData()
        {
            return LexingData.Instance.FourTokenSequences.Select(TestToken.GetSequenceData);
        }
    }
}
