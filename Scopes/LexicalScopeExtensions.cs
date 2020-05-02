using System;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public static class LexicalScopeExtensions
    {
        public static LexicalScope Assigned(this LexicalScope? scope)
        {
            return scope ?? throw new InvalidOperationException("Containing scope not assigned");
        }
    }
}
