using System;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class Parameter : IBindingMetadata
    {
        public bool IsMutableBinding { get; }
        public SimpleName Name { get; }
        public DataType Type { get; internal set; }

        Name IMetadata.FullName => throw new NotImplementedException();

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
