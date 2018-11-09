using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    /// <summary>
    /// This is the type of integer constants, it isn't possible to declare a
    /// variable to have this type. It will never be inferred as the type of a
    /// variable. It is un unresolved type because all expressions should have
    /// their types inferred to some specific type.
    /// </summary>
    public class IntegerConstantType : UnresolvedType
    {
        #region Singleton
        [NotNull] internal static readonly IntegerConstantType Instance = new IntegerConstantType();

        private IntegerConstantType() { }
        #endregion

        public override string ToString()
        {
            return "⧼integer-constant⧽";
        }
    }
}
