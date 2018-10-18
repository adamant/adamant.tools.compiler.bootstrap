using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class AccessModifierParser : IParser<AccessModifierSyntax>
    {
        [NotNull]
        public AccessModifierSyntax Parse([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            return new AccessModifierSyntax(tokens.Expect<IAccessModifierToken>());
        }
    }
}
