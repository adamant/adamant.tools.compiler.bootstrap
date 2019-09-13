using System;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(ExpressionSyntax),
        typeof(VariableDeclarationStatementSyntax))]
    public abstract class StatementSyntax : Syntax
    {
        private DataType type;
        public DataType Type
        {
            get => type;
            set
            {
                if (type != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                type = value ?? throw new ArgumentNullException(nameof(Type), "Can't set type to null");
            }
        }
    }
}
