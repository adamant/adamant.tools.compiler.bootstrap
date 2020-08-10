using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    public static class Primitive
    {
        public static readonly PrimitiveSymbolTree Instance = DefinePrimitiveSymbols();

        private static PrimitiveSymbolTree DefinePrimitiveSymbols()
        {
            // Primitives don't have a package
            var tree = new Dictionary<Symbol, FixedSet<Symbol>>();

            var stringType = new ObjectType(NamespaceName.Global, "String", false, ReferenceCapability.Shared);

            // Simple Types
            BuildBoolSymbol(tree);

            BuildIntegerTypeSymbol(tree, DataType.Byte, stringType);
            BuildIntegerTypeSymbol(tree, DataType.Int, stringType);
            BuildIntegerTypeSymbol(tree, DataType.UInt, stringType);

            BuildIntegerTypeSymbol(tree, DataType.Size, stringType);
            BuildIntegerTypeSymbol(tree, DataType.Offset, stringType);

            BuildEmptyType(tree, DataType.Void);
            BuildEmptyType(tree, DataType.Never);

            return new PrimitiveSymbolTree(tree.ToFixedDictionary());
        }

        private static void BuildBoolSymbol(Dictionary<Symbol, FixedSet<Symbol>> tree)
        {
            var symbol = new PrimitiveTypeSymbol(DataType.Bool);
            tree.Add(symbol, Children());
        }

        private static void BuildIntegerTypeSymbol(
            Dictionary<Symbol, FixedSet<Symbol>> tree,
            IntegerType integerType,
            DataType stringType)
        {
            var type = new PrimitiveTypeSymbol(integerType);

            var remainderMethod = new MethodSymbol(type, "remainder", integerType, Params(integerType), integerType);
            var displayStringMethod = new MethodSymbol(type, "to_display_string", integerType, Params(), stringType);

            tree.Add(type, Children(remainderMethod, displayStringMethod));
        }

        private static void BuildEmptyType(Dictionary<Symbol, FixedSet<Symbol>> tree, EmptyType emptyType)
        {
            var symbol = new PrimitiveTypeSymbol(emptyType);
            tree.Add(symbol, Children());
        }

        private static FixedList<DataType> Params(params DataType[] types)
        {
            return types.ToFixedList();
        }

        private static FixedSet<Symbol> Children(params Symbol[] symbols)
        {
            return symbols.ToFixedSet();
        }
    }
}
