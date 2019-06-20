namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The void type behaves similar to a unit type. However it represents the
    /// lack of a value. For example, a function returning `void` doesn't return
    /// a value. A parameter of type `void` is dropped from the parameter list.
    /// </summary>
    public class VoidType : SimpleType
    {
        #region Singleton
        internal static readonly VoidType Instance = new VoidType();

        private VoidType()
            : base("void")
        { }
        #endregion

        public override bool IsEmpty => true;

        public override bool IsKnown => true;
    }
}
