using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IIdentifierTokenPlace : IIdentifierOrPrimitiveTokenPlace, ILifetimeNameTokenPlace
    {
        [CanBeNull] string Value { get; }
    }
}
