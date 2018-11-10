using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public interface IUsingDirectiveParser
    {
        [MustUseReturnValue]
        [NotNull]
        SyntaxList<UsingDirectiveSyntax> ParseUsingDirectives(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics);
    }
}