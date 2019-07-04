using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class FieldDeclaration : Declaration
    {
        public bool MutableBinding { get; }

        public FieldDeclaration(bool mutableBinding, Name fullName, DataType type)
            : base(true, fullName, type, SymbolSet.Empty)
        {
            MutableBinding = mutableBinding;
        }
    }
}
