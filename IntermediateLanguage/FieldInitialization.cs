using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    /// <summary>
    /// A field initialization in an initialization caused by a constructor parameter
    /// </summary>
    public class FieldInitialization
    {
        public FieldSymbol Field { get; }

        public FieldInitialization(FieldSymbol field)
        {
            Field = field;
        }
    }
}
