using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Grammars
{
    public static class ChildrenGrammar
    {
        public static void Build(AttributeGrammar grammer)
        {
            grammer.For(Attribute.Parameters)
                .Rule<FunctionDeclarationSyntax>((f, children) =>
                    {
                        return children
                            .Where(c => c.SemanticNodeType() == typeof(Parameter))
                            .Cast<Attributes<ParameterSyntax>>()
                            .ToList()
                            .AsReadOnly();
                    });

            grammer.For(Attribute.ReturnType)
                .Rule<FunctionDeclarationSyntax>((f, children) =>
                {
                    return (Attributes<TypeSyntax>)children.Single(c => c.Syntax is TypeSyntax);
                });
        }
    }
}
