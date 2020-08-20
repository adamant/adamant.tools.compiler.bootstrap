using System;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public sealed class BoolConstantType : BoolType
    {
        internal new static readonly BoolConstantType True = new BoolConstantType(true);
        internal new static readonly BoolConstantType False = new BoolConstantType(false);

        public override bool IsConstant => true;

        public bool Value { get; }

        private BoolConstantType(bool value)
            : base(value ? SpecialTypeName.True : SpecialTypeName.False)
        {
            Value = value;
        }

        public override DataType ToNonConstantType()
        {
            return Bool;
        }

        public override string ToSourceCodeString()
        {
            throw new InvalidOperationException("Bool constant type has no source code representation");
        }

        public override string ToILString()
        {
            return $"const[{(Value ? "true" : "false")}]";
        }
    }
}
