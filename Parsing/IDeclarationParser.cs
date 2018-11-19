using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public interface IDeclarationParser
    {
        [MustUseReturnValue]
        [NotNull]
        NamespaceDeclarationSyntax ParseFileNamespace([NotNull] FixedList<string> name);

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        FixedList<DeclarationSyntax> ParseDeclarations();
    }
}
