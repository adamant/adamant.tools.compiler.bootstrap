using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// A symbol for a type declaration (i.e. a class)
    /// </summary>
    public sealed class TypeSymbol : ParentSymbol
    {
        public DataType DeclaresType { get; }

        public TypeSymbol(Name fullName, SymbolSet childSymbols, DataType declaresType)
            : base(fullName, childSymbols)
        {
            DeclaresType = declaresType;
        }

        public override bool Equals(Symbol? other)
        {
            throw new System.NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations", Justification = "<Pending>")]
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
