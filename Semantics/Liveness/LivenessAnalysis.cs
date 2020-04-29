using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness
{
    public class LivenessAnalysis : IBackwardDataFlowAnalysis<VariableFlags>
    {
        public VariableFlags StartState()
        {
            throw new NotImplementedException();
        }

        public VariableFlags Assignment(IAssignmentExpressionSyntax assignmentExpression, VariableFlags state)
        {
            throw new NotImplementedException();
        }

        public VariableFlags IdentifierName(INameExpressionSyntax nameExpression, VariableFlags state)
        {
            throw new NotImplementedException();
        }

        public VariableFlags VariableDeclaration(IVariableDeclarationStatementSyntax variableDeclaration, VariableFlags state)
        {
            throw new NotImplementedException();
        }
    }
}
