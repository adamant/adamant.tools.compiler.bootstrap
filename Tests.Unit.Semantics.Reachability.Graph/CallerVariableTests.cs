using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Reachability.Graph
{
    public class CallerVariableTests : SymbolTestFixture
    {
        [Fact]
        public void Self_parameter_to_string_has_name()
        {
            var sym = SelfParam(type: DataType("MyType", referenceCapability: ReferenceCapability.Borrowed));
            var v = new CallerVariable(new ReachabilityGraph(), sym);

            var actual = v.ToString();

            Assert.Equal("⟦self⟧: mut MyType", actual);
        }
    }
}
