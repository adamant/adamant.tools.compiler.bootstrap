using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    class ParameterLifetimeBoundSyntax : Syntax, IParameterLifetimeBoundSyntax
    {
        public SimpleName ParameterName { get; }

        public ParameterLifetimeBoundSyntax(TextSpan span, SimpleName parameterName)
            : base(span)
        {
            ParameterName = parameterName;
        }

        public override string ToString()
        {

            return $"${ParameterName}";
        }
    }
}
