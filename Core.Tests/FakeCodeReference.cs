namespace Adamant.Tools.Compiler.Bootstrap.Core.Tests
{
    public class FakeCodeReference : CodeReference
    {
        #region Singleton
        public static readonly FakeCodeReference Instance = new FakeCodeReference();

        private FakeCodeReference() { }
        #endregion
    }
}
