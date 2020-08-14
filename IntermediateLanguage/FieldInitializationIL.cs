using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    /// <summary>
    /// A field initialization in an initialization caused by a constructor parameter
    /// </summary>
    public class FieldInitializationIL
    {
        public FieldSymbol Field { get; }

        public FieldInitializationIL(FieldSymbol field)
        {
            Field = field;
        }
    }
}
