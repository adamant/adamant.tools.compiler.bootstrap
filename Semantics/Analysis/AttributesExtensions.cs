using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public static class AttributesExtensions
    {
        public static IReadOnlyList<Attributes<ParameterSyntax>> Parameters(
            this Attributes<FunctionDeclarationSyntax> f)
        {
            return f.Get(Attribute.Parameters);
        }

        public static Attributes<TypeSyntax> ReturnType(
            this Attributes<FunctionDeclarationSyntax> f)
        {
            return f.Get(Attribute.ReturnType);
        }
    }
}
