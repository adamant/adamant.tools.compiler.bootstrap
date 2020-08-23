using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class AbstractMethodDeclaration : InvocableDeclaration, IAbstractMethodDeclaration
    {
        public IClassDeclaration DeclaringClass { get; }
        public new MethodSymbol Symbol { get; }
        public ISelfParameter SelfParameter { get; }
        public new FixedList<INamedParameter> Parameters { get; }

        public AbstractMethodDeclaration(
            CodeFile file,
            TextSpan span,
            IClassDeclaration declaringClass,
            MethodSymbol symbol,
            TextSpan nameSpan,
            ISelfParameter selfParameter,
            FixedList<INamedParameter> parameters,
            IReachabilityAnnotations reachabilityAnnotations)
            : base(file, span, symbol, nameSpan, parameters.ToFixedList<IConstructorParameter>(), reachabilityAnnotations)
        {
            Symbol = symbol;
            SelfParameter = selfParameter;
            Parameters = parameters;
            DeclaringClass = declaringClass;
        }

        public override string ToString()
        {
            var returnType = Symbol.ReturnDataType != DataType.Void ? " -> " + Symbol.ReturnDataType : "";
            return $"fn {Symbol.Name}({string.Join(", ", Parameters.Prepend<IParameter>(SelfParameter))}){returnType};";
        }
    }
}
