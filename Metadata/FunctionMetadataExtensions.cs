using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    public static class FunctionMetadataExtensions
    {
        public static int Arity(this IFunctionMetadata function)
        {
            return function.Parameters.Count();
        }
    }
}
