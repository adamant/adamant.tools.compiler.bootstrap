using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FunctionInvocationExpressionSyntax : InvocationExpressionSyntax, IFunctionInvocationExpressionSyntax
    {
        public IInvocableNameSyntax FunctionNameSyntax { get; }
        public Promise<FunctionSymbol?> ReferencedSymbol { get; } = new Promise<FunctionSymbol?>();

        public FunctionInvocationExpressionSyntax(
            TextSpan span,
            Name name,
            IInvocableNameSyntax functionNameSyntax,
            FixedList<IArgumentSyntax> arguments)
            : base(span, name, functionNameSyntax.Name, arguments)
        {
            FunctionNameSyntax = functionNameSyntax;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{FullName}({string.Join(", ", Arguments)})";
        }
    }
}
