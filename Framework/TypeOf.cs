namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    /// <summary>
    /// This creates a way to pass generic types as regular arguments
    /// </summary>
    public struct TypeOf<T>
    {
        public static TypeOf<T> _;
    }
}
