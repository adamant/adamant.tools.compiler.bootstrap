using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FieldParameterSyntax : ParameterSyntax, IFieldParameterSyntax
    {
        private bool? isMutableBinding;
        public override bool IsMutableBinding => isMutableBinding ?? throw new Exception($"Tried to access {nameof(IsMutableBinding)} of field parameter before it had been assigned.");

        public SimpleName FieldName { get; }

        public IExpressionSyntax? DefaultValue { get; }

        public FieldParameterSyntax(
            TextSpan span,
            MaybeQualifiedName fullName,
            SimpleName fieldName,
            IExpressionSyntax? defaultValue)
            : base(span, fullName)
        {
            FieldName = fieldName;
            DefaultValue = defaultValue;
        }

        public void SetIsMutableBinding(bool value)
        {
            if (isMutableBinding != null) throw new InvalidOperationException("Can't set IsMutableBinding repeatedly");
            isMutableBinding = value;
        }

        public override string ToString()
        {
            var defaultValue = DefaultValue != null ? " = " + DefaultValue : "";
            return $".{Name}{defaultValue}";
        }
    }
}
