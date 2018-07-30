using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    internal class PackageSyntaxSymbol : SyntaxSymbol<PackageSyntax>, IPackageSyntaxSymbol
    {
        public GlobalNamespaceSyntaxSymbol GlobalNamespace { get; }
        IGlobalNamespaceSyntaxSymbol IPackageSyntaxSymbol.GlobalNamespace => GlobalNamespace;

        public PackageSyntax Declaration { get; }

        public PackageSyntaxSymbol(PackageSyntax package)
            : base("default", package) // TODO we should get name from the package syntax
        {
            GlobalNamespace = new GlobalNamespaceSyntaxSymbol(package);
            Declaration = package;
            AddChild(GlobalNamespace);
        }

        public IDeclarationSyntaxSymbol Lookup(VariableName variableName)
        {
            var scopeSymbol = Lookup(variableName.Scope);
            return (IDeclarationSyntaxSymbol)scopeSymbol?.Children.SingleOrDefault(c => c.Name == variableName.EntityName);
        }

        private ISyntaxSymbol Lookup(ScopeName scopeName)
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

        private IDeclarationSyntaxSymbol Lookup(FunctionName f)
        {
            var scopeSymbol = Lookup(f.Scope);
            return (IDeclarationSyntaxSymbol)scopeSymbol?.Children.SingleOrDefault(c => c.Name == f.EntityName);
        }

        private ISyntaxSymbol Lookup(NamedNamespaceName ns)
        {
            var scopeSymbol = Lookup(ns.ContainingNamespace);
            return (INamespaceSyntaxSymbol)scopeSymbol?.Children.SingleOrDefault(c => c.Name == ns.EntityName);
        }

        private ISyntaxSymbol Lookup(GlobalNamespaceName gns)
        {
            var packageSymbol = Lookup(gns.Package);
            return packageSymbol?.Children.Single(c => c.Name == "");
        }

        private IPackageSyntaxSymbol Lookup(PackageName packageName)
        {
            return packageName.EntityName == Name ? this : null;
        }
    }
}
