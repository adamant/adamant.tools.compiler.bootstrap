using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public abstract class Attribute
    {
        public static readonly InheritedAttribute<Type> SemanticNodeType = new InheritedAttribute<Type>("SemanticNodeType");
        public static readonly SynthesizedAttribute<DataType> Type = new SynthesizedAttribute<DataType>("Type");
        public static readonly SynthesizedAttribute<SemanticNode> SemanticNode = new SynthesizedAttribute<SemanticNode>("SemanticNode");

        // Child Attributes
        public static readonly SynthesizedAttribute<IReadOnlyList<Attributes<ParameterSyntax>>> Parameters = new SynthesizedAttribute<IReadOnlyList<Attributes<ParameterSyntax>>>("Parameters");
        public static readonly SynthesizedAttribute<Attributes<TypeSyntax>> ReturnType = new SynthesizedAttribute<Attributes<TypeSyntax>>("ReturnType");
    }
}
