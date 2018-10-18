using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public class ParameterAnalysis
    {
        [NotNull] public ParameterSyntax Syntax { get; }
        public bool MutableBinding { get; }
        [NotNull] public string Name { get; }
        [CanBeNull] public DataType Type { get; set; }

        public ParameterAnalysis([NotNull] ParameterSyntax syntax)
        {
            Syntax = syntax;
            MutableBinding = syntax.VarKeyword != null;
            Name = syntax.Name.Value ?? "_";
        }

        [NotNull]
        public Parameter Complete()
        {
            return new Parameter(MutableBinding, Name, Type.AssertNotNull());
        }
    }
}
