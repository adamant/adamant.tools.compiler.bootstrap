namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal enum ObjectState
    {
        /// <summary>
        /// Mutable by some reference, no read only references to it
        /// </summary>
        Mutable,
        /// <summary>
        /// One ore more read only references to it
        /// </summary>
        ReadOnly,
        /// <summary>
        /// Referenced by something like an identity but maybe nothing else
        /// </summary>
        Referenced,
        /// <summary>
        /// No references to it
        /// </summary>
        Released,
    }
}
