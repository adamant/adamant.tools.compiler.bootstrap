using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes
{
    internal class FakeCodeReference : CodeReference
    {
        #region Singleton

        public static readonly FakeCodeReference Instance = new FakeCodeReference();

        private FakeCodeReference()
            : base(FixedList<string>.Empty)
        { }
        #endregion

        public override string ToString()
        {
            throw new InvalidOperationException("Fake doesn't support ToString()");
        }
    }
}
