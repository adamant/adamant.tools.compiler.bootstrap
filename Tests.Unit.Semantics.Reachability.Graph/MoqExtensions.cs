using Adamant.Tools.Compiler.Bootstrap.Types;
using Moq.Language;
using Moq.Language.Flow;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Reachability.Graph
{
    public static class MoqExtensions
    {
        public static IReturnsResult<TMock> Returns<TMock>(this IReturns<TMock, TypePromise> mock, DataType type)
            where TMock : class
        {
            var promise = new TypePromise();
            promise.BeginFulfilling();
            promise.Fulfill(type);
            return mock.Returns(promise);
        }
    }
}
