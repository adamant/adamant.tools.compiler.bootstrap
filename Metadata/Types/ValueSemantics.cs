namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public enum ValueSemantics
    {
        /// <summary>
        /// This value type is a move type
        /// </summary>
        Move = 1,
        /// <summary>
        /// This value type is a copy type. Does not indicate whether it is safe
        /// to bit copy the type or a copy function is needed
        /// </summary>
        Copy,
    }
}
