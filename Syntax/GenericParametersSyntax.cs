using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GenericParametersSyntax : NonTerminal
    {
        [NotNull] public IOpenBracketTokenPlace OpenBracket { get; }
        [NotNull] public SeparatedListSyntax<GenericParameterSyntax> ParametersList { get; }
        [NotNull] [ItemNotNull] public IEnumerable<GenericParameterSyntax> Parameters => ParametersList.Nodes();
        [NotNull] public ICloseBracketTokenPlace CloseBracket { get; }

        public GenericParametersSyntax(
            [NotNull] IOpenBracketTokenPlace openBracket,
            [NotNull] SeparatedListSyntax<GenericParameterSyntax> parametersList,
            [NotNull] ICloseBracketTokenPlace closeBracket)
        {
            Requires.NotNull(nameof(openBracket), openBracket);
            Requires.NotNull(nameof(parametersList), parametersList);
            Requires.NotNull(nameof(closeBracket), closeBracket);
            OpenBracket = openBracket;
            ParametersList = parametersList;
            CloseBracket = closeBracket;
        }
    }
}
