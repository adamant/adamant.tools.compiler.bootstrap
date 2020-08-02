using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IClassDeclarationSyntax : ITypeMetadata
    {
        new DataTypePromise DeclaresType { get; }
        void CreateDefaultConstructor();
    }
}
