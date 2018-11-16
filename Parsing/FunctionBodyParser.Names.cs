using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class FunctionBodyParser : INameParser
    {
        [MustUseReturnValue]
        [NotNull]
        public NameSyntax ParseName()
        {
            NameSyntax name = ParseSimpleName();
            while (Tokens.Current is IDotToken)
            {
                var dot = Tokens.Take<IDotToken>();
                var simpleName = ParseSimpleName();
                name = new QualifiedNameSyntax(name, dot, simpleName);
            }
            return name;
        }

        [MustUseReturnValue]
        [NotNull]
        private SimpleNameSyntax ParseSimpleName()
        {
            var identifier = Tokens.ExpectIdentifier();
            SimpleNameSyntax simpleName;
            if (Tokens.Current is IOpenBracketToken)
            {
                var openBracket = Tokens.Consume<IOpenBracketTokenPlace>();
                var arguments = ParseArgumentList();
                var closeBracket = Tokens.Consume<ICloseBracketTokenPlace>();
                simpleName = new GenericNameSyntax(identifier, openBracket, arguments, closeBracket);
            }
            else
                simpleName = new IdentifierNameSyntax(identifier);
            return simpleName;
        }
    }
}
