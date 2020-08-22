using Adamant.Tools.Compiler.Bootstrap.AST;
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

        [Fact]
        public void Assign_replaces_references()
        {
            var graph = new ReferenceGraph();
            var v = graph.AddVariable(Variable("foo"));
            var mockSyntax = new Mock<IAbstractSyntax>();
            var ref1 = graph.AddNewObject(Ownership.Owns, Access.Mutable, mockSyntax.Object, false, false);
            v.AddReference(ref1);
            var mockExpression = new Mock<IExpression>();
            var ref2 = graph.AddNewObject(Ownership.Owns, Access.Mutable, mockExpression.Object, false, false);
            var temp = graph.AddTempValue(mockExpression.Object, DataType());
            temp.AddReference(ref2);

            v.Assign(temp);

            Assert.Equal(new[] { ref2 }, v.References);
        }
    }
}
