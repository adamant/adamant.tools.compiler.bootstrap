using System;
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
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        //typeof(OperatorDeclarationSyntax),
        //typeof(SetterDeclarationSyntax),
        typeof(NamedFunctionDeclarationSyntax),
        //typeof(InitializerDeclarationSyntax),
        //typeof(GetterDeclarationSyntax),
        //typeof(DestructorDeclarationSyntax),
        typeof(ConstructorDeclarationSyntax))]
    public abstract class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        private DataType selfParameterType;
        public DataType SelfParameterType
        {
            get => selfParameterType;
            set
            {
                if (selfParameterType != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                selfParameterType = value ?? throw new ArgumentException();
            }
        }

        public FixedList<IModiferToken> Modifiers { get; }
        //public FixedList<GenericParameterSyntax> GenericParameters { get; }
        public FixedList<ParameterSyntax> Parameters { get; } // For now we will not support pure meta functions
        //public FixedList<GenericConstraintSyntax> GenericConstraints { get; }
        //public FixedList<EffectSyntax> MayEffects { get; }
        //public FixedList<EffectSyntax> NoEffects { get; }
        //public FixedList<ExpressionSyntax> Requires { get; }
        //public FixedList<ExpressionSyntax> Ensures { get; }
        public BlockSyntax Body { get; }
        public TypePromise ReturnType { get; } = new TypePromise();
        public ControlFlowGraph ControlFlow { get; set; }

        protected FunctionDeclarationSyntax(
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            //FixedList<GenericParameterSyntax> genericParameters,
            FixedList<ParameterSyntax> parameters,
            //FixedList<GenericConstraintSyntax> genericConstraints,
            //FixedList<EffectSyntax> mayEffects,
            //FixedList<EffectSyntax> noEffects,
            //FixedList<ExpressionSyntax> requires,
            //FixedList<ExpressionSyntax> ensures,
            BlockSyntax body)
            : base(file, fullName, nameSpan,
                new SymbolSet(GetChildSymbols(/*genericParameters,*/ parameters, body)))
        {
            Modifiers = modifiers;
            Parameters = parameters;
            //MayEffects = mayEffects;
            //NoEffects = noEffects;
            //Requires = requires;
            //Ensures = ensures;
            Body = body;
            //GenericParameters = genericParameters;
            //GenericConstraints = genericConstraints;
        }

        private static IEnumerable<ISymbol> GetChildSymbols(
             //FixedList<GenericParameterSyntax> genericParameters,
             FixedList<ParameterSyntax> parameters,
             BlockSyntax body)
        {
            var variableDeclarations = GetVariableDeclarations(body);
            return ((IEnumerable<ISymbol>)parameters).Concat(variableDeclarations);
            //.Concat(genericParameters ?? Enumerable.Empty<ISymbol>())
            //.Concat(variableDeclarations);
        }

        private static IReadOnlyList<VariableDeclarationStatementSyntax> GetVariableDeclarations(BlockSyntax body)
        {
            var visitor = new GetVariableDeclarationsVisitor();
            visitor.VisitExpression(body, default);
            var variableDeclarations = visitor.VariableDeclarations;
            return variableDeclarations;
        }

        public IReadOnlyList<VariableDeclarationStatementSyntax> GetVariableDeclarations()
        {
            return GetVariableDeclarations(Body);
        }
    }
}
