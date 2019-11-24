using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class LoadStringInstruction : InstructionWithResult
    {
        public static readonly Encoding Encoding = new UTF8Encoding(false);

        public string Value { get; }

        public LoadStringInstruction(Place resultPlace, string value, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Value = value;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = LOAD \"{Value.Escape()}\"";
        }
    }
}
