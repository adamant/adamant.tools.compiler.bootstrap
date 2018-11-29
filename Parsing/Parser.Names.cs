using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        [MustUseReturnValue]
        [NotNull]
        public NameSyntax ParseName()
        {
            NameSyntax name = ParseSimpleName();
            while (Tokens.Accept<IDotToken>())
            {
                var simpleName = ParseSimpleName();
                name = new QualifiedNameSyntax(name, simpleName);
            }
            return name;
        }

        [MustUseReturnValue]
        [NotNull]
        private SimpleNameSyntax ParseSimpleName()
        {
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = new SimpleName(identifier.Value);
            SimpleNameSyntax syntax;
            if (Tokens.Accept<IOpenBracketToken>())
            {
                var arguments = ParseArguments();
                var closeBracket = Tokens.Expect<ICloseBracketToken>();
                var span = TextSpan.Covering(identifier.Span, closeBracket);
                syntax = new GenericNameSyntax(span, identifier.Value, arguments);
            }
            else
                syntax = new IdentifierNameSyntax(identifier.Span, name);
            return syntax;
        }
    }
}
