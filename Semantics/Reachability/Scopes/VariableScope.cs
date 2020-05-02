namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes
{
    internal abstract class VariableScope
    {
        public abstract CallerVariableScope CallerScope { get; }
    }
}
