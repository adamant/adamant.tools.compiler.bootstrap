using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Moq;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Reachability.Graph
{
    [Trait("Category", "Semantics")]
    public class VariableTests : SymbolTestFixture
    {
        [Fact]
        public void Self_parameter_to_string_has_name()
        {
            var sym = SelfParam(type: DataType("MyType", referenceCapability: ReferenceCapability.Borrowed));
            var mockGraph = new Mock<IReferenceGraph>();
            var v = new Variable(mockGraph.Object, sym);

            var actual = v.ToString();

            Assert.Equal("self: mut MyType", actual);
        }

        [Fact]
        public void ToString_include_mutable_binding()
        {
            var sym = Variable("foo", mut: true, type: DataType("MyType"));
            var mockGraph = new Mock<IReferenceGraph>();
            var v = new Variable(mockGraph.Object, sym);

            var actual = v.ToString();

            Assert.Equal("var foo: iso MyType", actual);
        }

        //[Fact]
        //public void Assign_replaces_references()
        //{
        //    var mockGraph = new Mock<IReferenceGraph>();
        //    var sym = Variable("foo", mut: true, type: DataType("MyType"));
        //    var v = new Variable(mockGraph.Object, sym);
        //    var mockReference = new Mock<IReference>();

        //    v.AddReference(mockReference.Object);

        //    var mockExp2 = new Mock<IExpression>();
        //    var newTemp = new TempValue(mockGraph.Object, mockExp2.Object, null!);
        //    var newReferences = newTemp.References;

        //    v.Assign(newTemp);

        //    Assert.Equal(newReferences, v.References);
        //}
    }
}
