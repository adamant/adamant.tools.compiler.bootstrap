using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Grammars
{
    public static class TypeGrammar
    {
        public static void Build(AttributeGrammar grammer)
        {
            grammer.For(Attribute.Type)
                .Rule<FunctionDeclarationSyntax>(f =>
                {
                    var parameterTypes = f.Parameters().Select(p => p.Type());
                    var returnType = f.ReturnType().Type();
                    return new FunctionType(parameterTypes, returnType);
                })
                .Rule<PrimitiveTypeSyntax>(p => PrimitiveType.New(p.Syntax.Keyword.Kind))
                .Value<ReturnExpressionSyntax>(PrimitiveType.Never);
        }
    }
}
