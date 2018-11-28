using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public class VoidType : SimpleType
    {
        #region Singleton
        [NotNull] internal static readonly VoidType Instance = new VoidType();

        private VoidType()
            : base("void")
        { }
        #endregion

        public override bool IsResolved => true;
    }
}
