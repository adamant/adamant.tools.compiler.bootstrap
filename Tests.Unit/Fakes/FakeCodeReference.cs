using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes
{
    public class FakeCodeReference : CodeReference
    {
        #region Singleton
        [NotNull]
        public static readonly FakeCodeReference Instance = new FakeCodeReference();

        private FakeCodeReference() { }
        #endregion
    }
}
