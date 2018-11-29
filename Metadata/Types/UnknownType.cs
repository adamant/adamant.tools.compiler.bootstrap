using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class UnknownType : UnresolvedType
    {
        #region Singleton
        [NotNull] internal static readonly UnknownType Instance = new UnknownType();

        private UnknownType() { }
        #endregion

        public override string ToString()
        {
            return "⧼unknown⧽";
        }
    }
}
