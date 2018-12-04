using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        public FixedList<IModiferToken> Modifiers { get; }
        public FixedList<GenericParameterSyntax> GenericParameters { get; }
        public FixedList<ParameterSyntax> Parameters { get; } // For now we will not support pure meta functions
        public FixedList<GenericConstraintSyntax> GenericConstraints { get; }
        public FixedList<EffectSyntax> MayEffects { get; }
        public FixedList<EffectSyntax> NoEffects { get; }
        public FixedList<ExpressionSyntax> Requires { get; }
        public FixedList<ExpressionSyntax> Ensures { get; }
        public BlockSyntax Body { get; }
        public TypePromise ReturnType { get; } = new TypePromise();
        public ControlFlowGraph ControlFlow { get; set; }

        protected FunctionDeclarationSyntax(
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<GenericParameterSyntax> genericParameters,
            FixedList<ParameterSyntax> parameters,
            FixedList<GenericConstraintSyntax> genericConstraints,
            FixedList<EffectSyntax> mayEffects,
            FixedList<EffectSyntax> noEffects,
            FixedList<ExpressionSyntax> requires,
            FixedList<ExpressionSyntax> ensures,
            BlockSyntax body)
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

        private static IEnumerable<ISymbol> GetChildSymbols(
             FixedList<GenericParameterSyntax> genericParameters,
             FixedList<ParameterSyntax> parameters,
             BlockSyntax body)
        {
            var visitor = new GetVariableDeclarationsVisitor();
            visitor.VisitExpression(body, default);
            return parameters
                .Concat(genericParameters ?? Enumerable.Empty<ISymbol>())
                .Concat(visitor.VariableDeclarations);
        }
    }
}
