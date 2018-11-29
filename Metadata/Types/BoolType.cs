using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public class BoolType : SimpleType
    {
        #region Singleton
        [NotNull] internal static readonly BoolType Instance = new BoolType();

        private BoolType()
            : base("bool")
        { }
        #endregion

        public override bool IsResolved => true;
    }
}
