using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class UnknownType : DataType
    {
        #region Singleton
        [NotNull]
        internal static readonly UnknownType Instance = new UnknownType();

        private UnknownType() { }
        #endregion

        [NotNull]
        public override string ToString()
        {
            return "⧼Unknown Type⧽";
        }
    }
}
