using System;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    internal static class LexicalScopeExtensions
    {
        public static LexicalScope Assigned(this LexicalScope? scope)
        {
            return scope ?? throw new InvalidOperationException("Containing scope not assigned");
        }
    }
}
