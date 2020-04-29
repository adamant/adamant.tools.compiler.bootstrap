using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ReferenceCapability;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    public class ReachabilityAnalyzer
    {
        private readonly IConcreteCallableDeclarationSyntax callableDeclaration;
        private readonly Diagnostics diagnostics;

        private ReachabilityAnalyzer(IConcreteCallableDeclarationSyntax callableDeclaration, Diagnostics diagnostics)
        {
            this.callableDeclaration = callableDeclaration;
            this.diagnostics = diagnostics;
        }

        public static void Check(FixedList<IConcreteCallableDeclarationSyntax> callableDeclarations, Diagnostics diagnostics)
        {
            foreach (var callableDeclaration in callableDeclarations)
                new ReachabilityAnalyzer(callableDeclaration, diagnostics).Check();
        }

        private void Check()
        {
            var parameterScope = CreateParameterScope();
            foreach (var statement in callableDeclaration.Body.Statements)
            {
                Check(statement, parameterScope);
            }
            // In control flow order:
            //    Create new variable scopes
            //    Update reachability graph based on expressions/statements
            //    Join graphs for control flow
            //    At end of scope for variable drops
            throw new NotImplementedException();
        }

        private static void Check(IStatementSyntax statement, VariableScope scope)
        {
            _ = scope;
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax stmt:
                    throw new NotImplementedException();
                case IExpressionStatementSyntax stmt:
                    throw new NotImplementedException();
                case IResultStatementSyntax stmt:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Create both the caller and parameter scope with the correct relationships
        /// between the parameters and the callers.
        /// </summary>
        private VariableScope CreateParameterScope()
        {
            var callerScope = new VariableScope(null);
            var parameterScope = new VariableScope(callerScope);
            foreach (var parameter in callableDeclaration.Parameters)
            {
                var parameterVariable = parameterScope.AddVariable(parameter.Name);
                // Non-reference types don't participate in reachability (yet)
                if (parameter.Type.Known() is ReferenceType referenceType)
                {

                    switch (referenceType.ReferenceCapability)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(referenceType.ReferenceCapability);
                        case IsolatedMutable:
                        case Isolated:
                        {
                            // Isolated parameters are fully independent of the caller
                            var referencedObject = new ObjectPlace();
                            parameterVariable.Owns(referencedObject);
                        }
                        break;
                        case Owned:
                        case OwnedMutable:
                        {
                            var referencedObject = new ObjectPlace();
                            parameterVariable.Owns(referencedObject);
                            var callerObject = NewCallerOwnedObject(callerScope, parameter);
                            referencedObject.Shares(callerObject);
                        }
                        break;
                        case Held:
                        case HeldMutable:
                        {
                            var referencedObject = new ObjectPlace();
                            parameterVariable.PotentiallyOwns(referencedObject);
                            var callerObject = NewCallerOwnedObject(callerScope, parameter);
                            referencedObject.Shares(callerObject);
                        }
                        break;
                        case Borrowed:
                        {
                            var callerObject = NewCallerOwnedObject(callerScope, parameter);
                            parameterVariable.Borrows(callerObject);
                        }
                        break;
                        case Shared:
                        {
                            var callerObject = NewCallerOwnedObject(callerScope, parameter);
                            parameterVariable.Shares(callerObject);
                        }
                        break;
                        case Identity:
                        {
                            var callerObject = NewCallerOwnedObject(callerScope, parameter);
                            parameterVariable.Identifies(callerObject);
                        }
                        break;
                    }
                }
            }

            return callerScope;
        }

        private static ObjectPlace NewCallerOwnedObject(VariableScope callerScope, IParameterSyntax parameter)
        {
            var callerVariable = callerScope.AddVariable(SpecialName.CallerBound(parameter.Name));
            var callerObject = new ObjectPlace();
            callerVariable.Owns(callerObject);
            return callerObject;
        }
    }
}
