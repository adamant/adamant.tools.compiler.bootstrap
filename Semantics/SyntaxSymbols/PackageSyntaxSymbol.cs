using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class PackageSyntaxSymbol : SyntaxSymbol
    {
        public SyntaxSymbol GlobalNamespace { get; }

        public PackageSyntaxSymbol(PackageSyntax declaration, SyntaxSymbol globalNamespace)
            // TODO use the real package name
            : base("default", null, declaration.Yield(), globalNamespace.Yield())
        {
            GlobalNamespace = globalNamespace;
        }

        public SyntaxSymbol Lookup(VariableName variableName)
        {
            var scopeSymbol = Lookup(variableName.Scope);
            return scopeSymbol?.Children.SingleOrDefault(c => c.Name == variableName.EntityName);
        }

        public SyntaxSymbol Lookup(ScopeName scopeName)
        {
            switch (scopeName)
            {
                case GlobalNamespaceName ns:
                    return Lookup(ns);
                case NamedNamespaceName ns:
                    return Lookup(ns);
                case FunctionName f:
                    return Lookup(f);
                default:
                    throw NonExhaustiveMatchException.For(scopeName);
            }
        }

        public SyntaxSymbol Lookup(FunctionName function)
        {
            var scopeSymbol = Lookup(function.Scope);
            return scopeSymbol?.Children.SingleOrDefault(c => c.Name == function.EntityName);
        }

        public SyntaxSymbol Lookup(NamedNamespaceName @namespace)
        {
            var scopeSymbol = Lookup(@namespace.ContainingNamespace);
            return scopeSymbol?.Children.SingleOrDefault(c => c.Name == @namespace.EntityName);
        }

        public SyntaxSymbol Lookup(GlobalNamespaceName globalNamespace)
        {
            var packageSymbol = Lookup(globalNamespace.Package);
            return packageSymbol?.Children.Single(c => c.Name == "");
        }

        public SyntaxSymbol Lookup(PackageName package)
        {
            return package.EntityName == Name ? this : null;
        }
    }
}
