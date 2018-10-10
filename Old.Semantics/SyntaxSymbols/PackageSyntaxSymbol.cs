using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class PackageSyntaxSymbol : ISyntaxSymbol
    {
        public string Name { get; }

        int? ISyntaxSymbol.DeclarationNumber => null;

        public PackageSyntax Declaration { get; }
        IEnumerable<SyntaxNode> ISyntaxSymbol.Declarations => Declaration.Yield();

        public GlobalNamespaceSyntaxSymbol GlobalNamespace { get; }
        IEnumerable<ISyntaxSymbol> ISyntaxSymbol.Children => GlobalNamespace.Yield();

        public PackageSyntaxSymbol(PackageSyntax declaration, GlobalNamespaceSyntaxSymbol globalNamespace)
        {
            // TODO use the real package name
            Name = "default";
            Declaration = declaration;
            GlobalNamespace = globalNamespace;
        }

        public VariableSyntaxSymbol Lookup(VariableName variableName)
        {
            var scopeSymbol = Lookup(variableName.Function);
            return scopeSymbol?.Children.OfType<VariableSyntaxSymbol>().SingleOrDefault(c => c.Name == variableName.EntityName);
        }

        public ISyntaxSymbol Lookup(ScopeName scopeName)
        {
            switch (scopeName)
            {
                case GlobalNamespaceName ns:
                    return Lookup(ns);
                case NamedNamespaceName ns:
                    return Lookup(ns);
                case FunctionName f:
                    return Lookup(f);
                case ReferenceTypeName rt:
                    return Lookup(rt);
                default:
                    throw NonExhaustiveMatchException.For(scopeName);
            }
        }

        public FunctionSyntaxSymbol Lookup(FunctionName function)
        {
            var scopeSymbol = Lookup(function.Scope);
            return scopeSymbol?.Children.OfType<FunctionSyntaxSymbol>().SingleOrDefault(c => c.Name == function.EntityName);
        }

        public NamespaceSyntaxSymbol Lookup(NamedNamespaceName @namespace)
        {
            var scopeSymbol = Lookup(@namespace.ContainingNamespace);
            return scopeSymbol?.Children.OfType<NamespaceSyntaxSymbol>().SingleOrDefault(c => c.Name == @namespace.EntityName);
        }

        public GlobalNamespaceSyntaxSymbol Lookup(GlobalNamespaceName globalNamespace)
        {
            var packageSymbol = Lookup(globalNamespace.Package);
            return packageSymbol?.GlobalNamespace;
        }

        public TypeSyntaxSymbol Lookup(ReferenceTypeName type)
        {
            var scopeSymbol = Lookup(type.Scope);
            return scopeSymbol?.Children.OfType<TypeSyntaxSymbol>().SingleOrDefault(c => c.Name == type.EntityName);
        }

        public PackageSyntaxSymbol Lookup(PackageName package)
        {
            return package.EntityName == Name ? this : null;
        }
    }
}
