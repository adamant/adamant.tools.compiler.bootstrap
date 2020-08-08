using System;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public static class ScopeExtensions
    {
        public static Scope Assigned(this Scope? scope)
        {
            return scope ?? throw new InvalidOperationException("Containing scope not assigned");
        }
    }
}
