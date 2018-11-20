using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public interface IUsingDirectiveParser
    {
        [MustUseReturnValue]
        [NotNull]
        FixedList<UsingDirectiveSyntax> ParseUsingDirectives();
    }
}
