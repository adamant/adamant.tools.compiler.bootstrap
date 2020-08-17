using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class FunctionDeclaration : InvocableDeclaration, IFunctionDeclaration
    {
        public new FunctionSymbol Symbol { get; }
        public new FixedList<INamedParameter> Parameters { get; }
        public IBody Body { get; }

        public FunctionDeclaration(
            CodeFile file,
            TextSpan span,
            FunctionSymbol symbol,
            TextSpan nameSpan,
            FixedList<INamedParameter> parameters,
            IBody body)
            : base(file, span, symbol, nameSpan, parameters.ToFixedList<IConstructorParameter>())
        {
            Symbol = symbol;
            Parameters = parameters;
            Body = body;
        }

        public override string ToString()
        {
            var returnType = Symbol.ReturnDataType != DataType.Void ? " -> " + Symbol.ReturnDataType : "";
            return $"fn {Symbol.ContainingSymbol}.{Symbol.Name}({string.Join(", ", Parameters)}){returnType} {Body}";
        }
    }
}
