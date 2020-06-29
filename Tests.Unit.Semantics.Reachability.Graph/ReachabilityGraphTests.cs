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
        public void AddValueTypeParameter()
        {
            var graph = new ReachabilityGraph();
            var mockParameter = new Mock<IParameterSyntax>();
            mockParameter.Setup(p => p.Type).Returns(DataType.Bool);

            graph.AddParameter(mockParameter.Object);

            Assert.Empty(graph.CallerVariables);
            Assert.Empty(graph.Variables);
        }
    }
}
