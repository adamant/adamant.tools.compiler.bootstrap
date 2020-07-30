using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    public static class FunctionSymbolExtensions
    {
        public static int Arity(this IFunctionSymbol function)
        {
            return function.Parameters.Count();
        }
    }
}
