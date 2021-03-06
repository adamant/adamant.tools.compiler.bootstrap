using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Moq;
using Xunit;


namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Reachability.Graph
{
    [Trait("Category", "Semantics")]
    public class ReachabilityGraphTests
    {
        [Fact]
        public void AddParameterWithValueType()
        {
            var graph = new ReachabilityGraph();
            var mockParameter = new Mock<IBindingParameter>();
            var fakeFuncSymbol = new FunctionSymbol(new PackageSymbol("my.package"), "func", FixedList<DataType>.Empty, DataType.Void);
            var fakeVariableSymbol = new VariableSymbol(fakeFuncSymbol, "var_name", null, false, DataType.Bool);
            mockParameter.Setup(p => p.Symbol).Returns(fakeVariableSymbol);

            var localVariable = graph.AddParameter(mockParameter.Object);

            Assert.Null(localVariable);
            Assert.Empty(graph.ReferenceGraph.CallerVariables);
            Assert.Empty(graph.ReferenceGraph.Variables);
        }

        // TODO Unit test all the other AddParameter cases

        // TODO Unit test AddField

        // TODO Unit test AddVariable

        [Fact]
        public void AddNewObjectWithValueType()
        {
            var graph = new ReachabilityGraph();
            var mockExpression = new Mock<INewObjectExpression>();
            mockExpression.Setup(e => e.DataType).Returns(DataType.Int);

            var temp = graph.AddObject(mockExpression.Object);

            Assert.Null(temp);
            Assert.Empty(graph.ReferenceGraph.Objects);
            Assert.Empty(graph.ReferenceGraph.TempValues);
        }

        [Fact]
        public void AddNewObject()
        {
            var graph = new ReachabilityGraph();
            var mockExpression = new Mock<INewObjectExpression>();
            // TODO this type should be Isolated or Owned
            var fakeReferenceType = new ObjectType(NamespaceName.Global, "Fake", false, ReferenceCapability.Shared);
            mockExpression.Setup(e => e.DataType).Returns(fakeReferenceType);

            var temp = graph.AddObject(mockExpression.Object);

            Assert.NotNull(temp);
            Assert.Collection(graph.ReferenceGraph.Objects, o =>
            {
                Assert.Equal(mockExpression.Object, o.OriginSyntax);
                Assert.Single(temp!.PossibleReferents, o);
                // TODO what else to test
            });
            Assert.Contains(temp, graph.ReferenceGraph.TempValues);
        }

        [Fact]
        public void AssignTempToVariable()
        {
            var graph = new ReachabilityGraph();
            var mockExpression = new Mock<INewObjectExpression>();
            // TODO this type should be Isolated or Owned
            var fakeReferenceType = new ObjectType(NamespaceName.Global, "Fake", false, ReferenceCapability.Shared);
            mockExpression.Setup(e => e.DataType).Returns(fakeReferenceType);
            var temp = graph.AddObject(mockExpression.Object);
            var mockVariable = new Mock<IVariableDeclarationStatement>();
            var funcSymbol = new FunctionSymbol(new PackageSymbol("my.package"), "func", FixedList<DataType>.Empty, DataType.Void);
            var fakeVariableSymbol = new VariableSymbol(funcSymbol, "var_name", null, false, fakeReferenceType);
            mockVariable.Setup(v => v.Symbol).Returns(fakeVariableSymbol);
            var variable = graph.AddVariable(mockVariable.Object);

            graph.Assign(variable, temp);

            Assert.DoesNotContain(temp, graph.ReferenceGraph.TempValues);
        }
    }
}
