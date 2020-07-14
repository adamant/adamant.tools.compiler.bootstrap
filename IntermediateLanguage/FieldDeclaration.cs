using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class FieldDeclaration : Declaration, IBindingSymbol
    {
        public bool IsMutableBinding { get; }
        public DataType Type { get; }

        public FieldDeclaration(bool isMutableBinding, Name fullName, DataType type)
            : base(true, fullName, SymbolSet.Empty)
        {
            IsMutableBinding = isMutableBinding;
            Type = type;
        }
    }
}
