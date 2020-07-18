using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// A symbol for a function
    /// </summary>
    public class FunctionSymbol : ParentSymbol
    {
        public FixedList<BindingSymbol> Parameters { get; }
        public int Arity => Parameters.Count;
        public DataType ReturnType { get; }

        // TODO isn't there overlap between the parameters and the child symbols?
        public FunctionSymbol(
            Name fullName,
            FixedList<BindingSymbol> parameters,
            DataType returnType,
            SymbolSet childSymbols)
            : base(fullName, childSymbols)
        {
            Parameters = parameters;
            ReturnType = returnType;
        }

        public static FunctionSymbol CreateDefaultConstructor(ObjectType constructedType)
        {
            var name = constructedType.FullName.Qualify(SpecialName.New);
            return new FunctionSymbol(name, FixedList<BindingSymbol>.Empty, constructedType, SymbolSet.Empty);
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            // Check exact type to make sure it isn't a method
            if (other.GetType() != GetType()) return false;
            var otherSymbol = (FunctionSymbol)other;
            return FullName == otherSymbol.FullName
                && Parameters.SequenceEqual(otherSymbol.Parameters)
                && ReturnType == otherSymbol.ReturnType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FullName, Parameters, ReturnType);
        }
    }
}
