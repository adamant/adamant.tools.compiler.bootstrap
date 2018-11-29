using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public class TypeType : ReferenceType
    {
        #region Singleton
        [NotNull] internal static readonly TypeType Instance = new TypeType();

        private TypeType() { }
        #endregion

        public override bool IsResolved => true;

        public override string ToString() => "Type";
    }
}
