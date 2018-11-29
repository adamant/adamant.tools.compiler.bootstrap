using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class NeverType : SimpleType
    {
        #region Singleton
        [NotNull] internal static readonly NeverType Instance = new NeverType();

        private NeverType()
            : base("never")
        { }
        #endregion

        public override bool IsResolved => true;
    }
}
