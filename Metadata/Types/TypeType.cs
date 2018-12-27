using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class TypeType : ReferenceType
    {
        #region Singleton
        internal static readonly TypeType Instance = new TypeType();

        private TypeType()
            : base(Lifetimes.Lifetime.None)
        { }
        #endregion

        public override bool IsResolved => true;

        public override ReferenceType WithLifetime(Lifetime lifetime)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => "Type";
    }
}
