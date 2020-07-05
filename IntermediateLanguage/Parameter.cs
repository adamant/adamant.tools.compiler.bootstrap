using System;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class Parameter : IBindingSymbol
    {
        public bool IsMutableBinding { get; }
        public SimpleName Name { get; }
        public DataType Type { get; internal set; }

        Name ISymbol.FullName => throw new NotImplementedException();

        public Parameter(
            bool isMutableBinding,
            SimpleName name,
            DataType type)
        {
            IsMutableBinding = isMutableBinding;
            Name = name;
            Type = type;
        }
    }
}
