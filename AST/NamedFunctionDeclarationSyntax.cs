using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NamedFunctionDeclarationSyntax : FunctionDeclarationSyntax, INamespacedDeclarationSyntax
    {
        public ExpressionSyntax ReturnTypeExpression { get; }
        DataType ISymbol.Type => Type.Fulfilled();
        public bool IsExternalFunction { get; set; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DeclarationSyntax IDeclarationSyntax.AsDeclarationSyntax => this;

        public NamedFunctionDeclarationSyntax(
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<GenericParameterSyntax> genericParameters,
            FixedList<ParameterSyntax> parameters, // For now we will not support pure meta functions
            ExpressionSyntax returnTypeExpression,
            FixedList<GenericConstraintSyntax> genericConstraints,
            FixedList<EffectSyntax> mayEffects,
            FixedList<EffectSyntax> noEffects,
            FixedList<ExpressionSyntax> requires,
            FixedList<ExpressionSyntax> ensures,
            BlockSyntax body)
            : base(file, modifiers, fullName, nameSpan, genericParameters, parameters,
                genericConstraints, mayEffects, noEffects, requires, ensures, body)
        {
            ReturnTypeExpression = returnTypeExpression;
        }

        public override string ToString()
        {
            var generics = GenericParameters != null
                ? $"[{string.Join(", ", GenericParameters)}]"
                : "";
            var returnType = ReturnTypeExpression != null ? " -> " + ReturnTypeExpression : "";
            var body = Body != null ? " {{ … }}" : ";";
            return $"fn {FullName}{generics}({string.Join(", ", Parameters)}){returnType}{body}";
        }
    }
}
