namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal enum ObjectState
    {
        Mutable,
        ReadOnly,
        /// <summary>
        /// Referenced by something like and identity but maybe nothing else
        /// </summary>
        Referenced,
        Released,
    }
}
