using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    public static class Primitive
    {
        public static readonly PrimitiveSymbolTree SymbolTree = DefinePrimitiveSymbols();

        private static PrimitiveSymbolTree DefinePrimitiveSymbols()
        {
            var tree = new SymbolTreeBuilder();

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

            return tree.BuildPrimitives();
        }

        private static void BuildBoolSymbol(SymbolTreeBuilder tree)
        {
            var symbol = new PrimitiveTypeSymbol(DataType.Bool);
            tree.Add(symbol);
        }

        private static void BuildIntegerTypeSymbol(
            SymbolTreeBuilder tree,
            IntegerType integerType,
            DataType stringType)
        {
            var type = new PrimitiveTypeSymbol(integerType);
            tree.Add(type);

            var remainderMethod = new MethodSymbol(type, "remainder", integerType, Params(integerType), integerType);
            var displayStringMethod = new MethodSymbol(type, "to_display_string", integerType, Params(), stringType);

            tree.Add(remainderMethod);
            tree.Add(displayStringMethod);
        }

        private static void BuildEmptyType(SymbolTreeBuilder tree, EmptyType emptyType)
        {
            var symbol = new PrimitiveTypeSymbol(emptyType);
            tree.Add(symbol);
        }

        private static FixedList<DataType> Params(params DataType[] types)
        {
            return types.ToFixedList();
        }
    }
}
