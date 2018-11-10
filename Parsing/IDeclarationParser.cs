using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
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
