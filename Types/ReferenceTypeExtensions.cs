namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public static class ReferenceTypeExtensions
    {
        /// <summary>
        /// Return the same type except with the given reference capability
        /// </summary>
        public static T To<T>(this T type, ReferenceCapability referenceCapability)
            where T : ReferenceType
        {
            return type.To_ReturnsSelf(referenceCapability).Cast<T>();
        }
    }
}
