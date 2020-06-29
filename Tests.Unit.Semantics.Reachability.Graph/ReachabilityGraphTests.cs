using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Reachability.Graph
{
    [UnitTest]
    [Category("Semantic")]
    public class ReachabilityGraphTests
    {
        [Fact]
        public void AddParameterWithValueType()
        {
            var graph = new ReachabilityGraph();
            var mockParameter = new Mock<IParameterSyntax>();
            mockParameter.Setup(p => p.Type).Returns(DataType.Bool);

            var localVariable = graph.AddParameter(mockParameter.Object);

            Assert.Null(localVariable);
            Assert.Empty(graph.CallerVariables);
            Assert.Empty(graph.Variables);
        }

        // TODO Unit test all the other AddParameter cases

        // TODO Unit test AddField

        // TODO Unit test AddVariable
    }
}
