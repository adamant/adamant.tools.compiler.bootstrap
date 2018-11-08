using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    // Represents a type that is in the process of being checked/computed. Used
    // to check if there a type cycles.
    public class BeingCheckedType : DataType
    {
        #region Singleton
        [NotNull] public static readonly BeingCheckedType Instance = new BeingCheckedType();

        private BeingCheckedType() { }
        #endregion

        public override string ToString()
        {
            return "⧼being-checked⧽";
        }
    }
}
