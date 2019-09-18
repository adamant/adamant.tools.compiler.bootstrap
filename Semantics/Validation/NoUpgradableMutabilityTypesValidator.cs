using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;
using Void = Adamant.Tools.Compiler.Bootstrap.Framework.Void;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    /// <summary>
    /// No variable types, parameter types, return types should have any
    /// implicitly mutable types in them.
    /// </summary>
    public class NoUpgradableMutabilityTypesValidator : DeclarationVisitor<Void>
    {
        public static void Validate(IEnumerable<MemberDeclarationSyntax> allMemberDeclarations)
        {
            var validator = new NoUpgradableMutabilityTypesValidator();
            foreach (var declaration in allMemberDeclarations)
                validator.VisitDeclaration(declaration);
        }

        public override void VisitFunctionDeclaration(FunctionDeclarationSyntax functionDeclaration, Void args)
        {
            base.VisitFunctionDeclaration(functionDeclaration, args);
            Validate(functionDeclaration.SelfParameterType);
            ValidateReturn(functionDeclaration.ReturnType.Fulfilled());
        }

        public override void VisitParameter(ParameterSyntax parameter, Void args)
        {
            Validate(parameter.Type.Fulfilled());
        }

        public override void VisitVariableDeclarationStatement(
            VariableDeclarationStatementSyntax variableDeclaration,
            Void args)
        {
            base.VisitVariableDeclarationStatement(variableDeclaration, args);
            Validate(variableDeclaration.Type);
        }

        private static void Validate(DataType type)
        {
            switch (type)
            {
                case ReferenceType referenceType:
                    if (referenceType.Mutability.IsUpgradable)
                        throw new Exception($"Type is upgradable `{referenceType}` and shouldn't be");
                    break;
                case OptionalType optionalType:
                    Validate(optionalType.Referent);
                    break;
                //case RefType refType:
                //    Validate(refType.Referent);
                //    break;
                case EmptyType _:
                case SimpleType _:
                case UnknownType _:
                case PointerType _:
                    // can't contain a reference type
                    break;
                case null:
                    // Nothing to check
                    break;
                default:
                    throw ExhaustiveMatch.Failed(type);
            }
        }

        private static void ValidateReturn(DataType type)
        {
            switch (type)
            {
                case ReferenceType referenceType:
                    // Owned returns are supposed to be upgradable so that the caller
                    // can receive ownership.
                    if (referenceType.Mutability.IsUpgradable && !referenceType.IsOwned)
                        throw new Exception($"Type is upgradable `{referenceType}` and shouldn't be");
                    break;
                case OptionalType optionalType:
                    Validate(optionalType.Referent);
                    break;
                //case RefType refType:
                //    Validate(refType.Referent);
                //    break;
                case EmptyType _:
                case SimpleType _:
                case UnknownType _:
                case PointerType _:
                    // can't contain a reference type
                    break;
                case null:
                    // Nothing to check
                    break;
                default:
                    throw ExhaustiveMatch.Failed(type);
            }
        }
    }
}
