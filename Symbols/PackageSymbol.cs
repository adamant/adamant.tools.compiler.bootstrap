using System;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// A symbol a package
    /// </summary>
    /// <remarks>
    /// A package alias has no affect on the symbol. It is still the same package.
    /// </remarks>
    public class PackageSymbol : NamespaceOrPackageSymbol
    {
        public PackageSymbol(Name name)
            : base(null, name) { }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is PackageSymbol otherNamespace
                   && Name == otherNamespace.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
