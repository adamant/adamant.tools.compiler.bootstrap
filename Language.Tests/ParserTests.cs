using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetAllParseTestCases))]
        public void Parses(ParseTestCase testCase)
        {
            // TODO
        }

        public static TheoryData<ParseTestCase> GetAllParseTestCases()
        {
            // TODO
            var theoryData = new TheoryData<ParseTestCase>();
            theoryData.Add(new ParseTestCase());
            return theoryData;
        }
    }
}
