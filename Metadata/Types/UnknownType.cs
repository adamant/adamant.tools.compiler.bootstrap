using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class UnknownType : DataType
    {
        #region Singleton
        [NotNull] internal static readonly UnknownType Instance = new UnknownType();

        private UnknownType() { }
        #endregion

        public override bool IsResolved => false;

        public override string ToString()
        {
            return "⧼unknown⧽";
        }
    }
}
