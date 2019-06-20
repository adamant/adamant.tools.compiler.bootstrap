using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class TypeType : ObjectType
    {
        #region Singleton
        internal static readonly TypeType Instance = new TypeType();

        private TypeType()
            : base(Lifetime.None)
        { }
        #endregion

        public override bool IsKnown => true;

        public override ReferenceType WithLifetime(Lifetime lifetime)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => "Type";
    }
}
