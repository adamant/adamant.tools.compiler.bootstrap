using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public class AnyType : ReferenceType
    {
        #region Singleton
        [NotNull] internal static readonly AnyType Instance = new AnyType();

        private AnyType() { }
        #endregion

        public override bool IsResolved => true;

        public override string ToString() => "Any";
    }
}
