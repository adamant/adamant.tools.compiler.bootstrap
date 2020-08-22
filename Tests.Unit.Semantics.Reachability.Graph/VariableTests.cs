using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Reachability.Graph
{
    public class VariableTests : SymbolTestFixture
    {
        [Fact]
        public void Self_parameter_to_string_has_name()
        {
            var sym = SelfParam(type: DataType("MyType", referenceCapability: ReferenceCapability.Borrowed));
            var v = new Variable(new ReachabilityGraph(), sym);

            var actual = v.ToString();

            Assert.Equal("self: mut MyType", actual);
        }

        [Fact]
        public void ToString_include_mutable_binding()
        {
            var sym = Variable("foo", mut: true, type: DataType("MyType"));
            var v = new Variable(new ReachabilityGraph(), sym);

            var actual = v.ToString();

            Assert.Equal("var foo: iso MyType", actual);
        }
    }
}
