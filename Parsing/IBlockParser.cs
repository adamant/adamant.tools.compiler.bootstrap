using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public interface IBlockParser
    {
        [MustUseReturnValue]
        [CanBeNull]
        BlockSyntax AcceptBlock();

        [NotNull]
        [MustUseReturnValue]
        BlockSyntax ParseBlock();
    }
}
