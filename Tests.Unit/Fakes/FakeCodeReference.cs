using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes
{
    internal class FakeCodeReference : CodeReference
    {
        #region Singleton
        [NotNull]
        public static readonly FakeCodeReference Instance = new FakeCodeReference();

        private FakeCodeReference()
            : base(FixedList<string>.Empty)
        { }
        #endregion
    }
}
