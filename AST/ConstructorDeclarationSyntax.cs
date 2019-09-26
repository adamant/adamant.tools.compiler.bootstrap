using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ConstructorDeclarationSyntax : FunctionDeclarationSyntax
    {
        public override FixedList<StatementSyntax> Body => base.Body ?? throw new InvalidOperationException();

        public ConstructorDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<ParameterSyntax> parameters,
            FixedList<StatementSyntax> body)
            : base(span, file, modifiers, fullName, nameSpan, parameters, body)
        {
        }

        public override string ToString()
        {
            return $"{FullName}({string.Join(", ", Parameters)})";
        }
    }
}
