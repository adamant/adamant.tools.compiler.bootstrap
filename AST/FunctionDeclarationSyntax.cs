using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public FixedList<IModiferToken> Modifiers { get; }
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [NotNull] public FixedList<ParameterSyntax> Parameters { get; } // For now we will not support pure meta functions
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public FixedList<EffectSyntax> MayEffects { get; }
        [NotNull] public FixedList<EffectSyntax> NoEffects { get; }
        [NotNull] public FixedList<ExpressionSyntax> Requires { get; }
        [NotNull] public FixedList<ExpressionSyntax> Ensures { get; }
        [CanBeNull] public BlockSyntax Body { get; }
        [NotNull] public TypePromise ReturnType { get; } = new TypePromise();
        [CanBeNull] public ControlFlowGraph ControlFlow { get; set; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        protected FunctionDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] Name fullName,
            TextSpan nameSpan,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(file, fullName, nameSpan,
                new SymbolSet(GetChildSymbols(genericParameters, parameters, body)))
        {
            Modifiers = modifiers;
            Parameters = parameters;
            MayEffects = mayEffects;
            NoEffects = noEffects;
            Requires = requires;
            Ensures = ensures;
            Body = body;
            GenericParameters = genericParameters;
            GenericConstraints = genericConstraints;
        }

        [NotNull]
        private static IEnumerable<ISymbol> GetChildSymbols(
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [CanBeNull] BlockSyntax body)
        {
            var visitor = new GetVariableDeclarationsVisitor();
            visitor.VisitExpression(body, default);
            return parameters
                .Concat(genericParameters ?? Enumerable.Empty<ISymbol>())
                .Concat(visitor.VariableDeclarations);
        }

        protected override DataType GetDataType()
        {
            return Type.Fulfilled();
        }
    }
}
