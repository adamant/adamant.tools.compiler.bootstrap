using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using ValueType = Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ValueType;
using Void = Adamant.Tools.Compiler.Bootstrap.Framework.Void;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Moving
{
    /// <summary>
    /// Computes ValueSemantics for all expression
    /// </summary>
    public class MoveChecker : ExpressionVisitor<Void>
    {
        private readonly FunctionDeclarationSyntax function;
        private readonly Diagnostics diagnostics;

        private MoveChecker(FunctionDeclarationSyntax function, Diagnostics diagnostics)
        {
            this.function = function;
            this.diagnostics = diagnostics;
        }

        public static void Check(IEnumerable<MemberDeclarationSyntax> memberDeclarations, Diagnostics diagnostics)
        {
            foreach (var declaration in memberDeclarations.OfType<FunctionDeclarationSyntax>())
                Check(declaration, diagnostics);
        }

        private static void Check(FunctionDeclarationSyntax function, Diagnostics diagnostics)
        {
            if (function.Body == null) return;

            var moveChecker = new MoveChecker(function, diagnostics);
            moveChecker.VisitExpression(function.Body);
        }

        public override void VisitExpression(ExpressionSyntax expression, Void args)
        {
            base.VisitExpression(expression, args);
            // If it already has a value mode, we are good
            if (expression == null || expression.ValueSemantics != null) return;

            var type = expression.Type;
            switch (type)
            {
                case NeverType _:
                    expression.ValueSemantics = ValueSemantics.Never;
                    break;
                case ReferenceType _:
                case UnknownType _:
                case VoidType _:
                    expression.ValueSemantics = ValueSemantics.Copy;
                    break;
                case ValueType valueType:
                    expression.ValueSemantics = valueType.Semantics;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(type);
            }
        }

        public override void VisitMoveExpression(MoveExpressionSyntax moveExpression, Void args)
        {
            base.VisitMoveExpression(moveExpression, args);
            // TODO do we need to check this is something valid to move out of?
            moveExpression.Expression.ValueSemantics = ValueSemantics.Move;
            moveExpression.ValueSemantics = ValueSemantics.Move;
        }

        public override void VisitMutableExpression(MutableExpressionSyntax mutableExpression, Void args)
        {
            base.VisitMutableExpression(mutableExpression, args);

            mutableExpression.ValueSemantics = mutableExpression.Expression.ValueSemantics;
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpression, Void args)
        {
            base.VisitAssignmentExpression(assignmentExpression, args);
            var assignToType = assignmentExpression.LeftOperand.Type;
            var value = assignmentExpression.RightOperand;
            CheckAssignment(assignToType, value);
        }

        public override void VisitVariableDeclarationStatement(
            VariableDeclarationStatementSyntax variableDeclaration,
            Void args)
        {
            base.VisitVariableDeclarationStatement(variableDeclaration, args);
            var assignToType = variableDeclaration.Type;
            var value = variableDeclaration.Initializer;
            if (value != null)
                CheckAssignment(assignToType, value);
        }

        private void CheckAssignment(DataType assignToType, ExpressionSyntax value)
        {
            if (assignToType is ReferenceType referenceType && referenceType.IsOwned)
            {
                if (value.ValueSemantics != ValueSemantics.Move)
                {
                    diagnostics.Add(SemanticError.CantMove(function.File, value));
                    function.MarkErrored();
                }
            }
        }

        public override void VisitInvocation(InvocationSyntax invocation, Void args)
        {
            base.VisitInvocation(invocation, args);

            CheckArguments(invocation.Callee.Type, invocation.Arguments);

            // Functions returning ownership are effectively move
            if (invocation.Type is ReferenceType returnType && returnType.IsOwned)
                invocation.ValueSemantics = ValueSemantics.Move;
        }

        public override void VisitNewObjectExpression(NewObjectExpressionSyntax newObjectExpression, Void args)
        {
            base.VisitNewObjectExpression(newObjectExpression, args);

            CheckArguments(newObjectExpression.ConstructorType, newObjectExpression.Arguments);
            newObjectExpression.ValueSemantics = ValueSemantics.Move;
        }

        private void CheckArguments(DataType calleeType, FixedList<ArgumentSyntax> arguments)
        {
            if (calleeType is UnknownType) return;
            // TODO what if it isn't a function type?
            var parameterTypes = ((FunctionType)calleeType).ParameterTypes;

            foreach (var (type, argument) in parameterTypes.Zip(arguments))
                CheckAssignment(type, argument.Value);
        }

        public override void VisitImplicitImmutabilityConversionExpression(
            ImplicitImmutabilityConversionExpression conversionExpression,
            Void args)
        {
            base.VisitImplicitImmutabilityConversionExpression(conversionExpression, args);
            conversionExpression.ValueSemantics = conversionExpression.Expression.ValueSemantics;
        }
    }
}
