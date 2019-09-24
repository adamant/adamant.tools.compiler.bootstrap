using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public static class FunctionSymbolExtensions
    {
        public static int Arity(this IFunctionSymbol function)
        {
            return function.Parameters.Count();
        }
    }
}
