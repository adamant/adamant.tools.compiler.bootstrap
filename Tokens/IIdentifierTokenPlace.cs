using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IIdentifierTokenPlace : ILifetimeNameTokenPlace
    {
        [CanBeNull] string Value { get; }
    }
}
