using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
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
