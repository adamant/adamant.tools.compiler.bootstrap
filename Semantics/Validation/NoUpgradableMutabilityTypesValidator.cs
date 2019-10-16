using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    /// <summary>
    /// No variable types, parameter types, return types should have any
    /// implicitly mutable types in them.
    /// </summary>
    public class NoUpgradableMutabilityTypesValidator : SyntaxWalker
    {
        public void Walk(IEnumerable<IEntityDeclarationSyntax> entityDeclarations)
        {
            foreach (var declaration in entityDeclarations) WalkNonNull(declaration);
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IMethodDeclarationSyntax methodDeclaration:
                    WalkChildren(methodDeclaration);
                    Validate(methodDeclaration.SelfParameterType!);
                    ValidateReturn(methodDeclaration.ReturnType.Fulfilled());
                    break;
                case IConstructorDeclarationSyntax constructorDeclaration:
                    WalkChildren(constructorDeclaration);
                    Validate(constructorDeclaration.SelfParameterType);
                    break;
                case IParameterSyntax parameter:
                    WalkChildren(parameter);
                    Validate(parameter.Type.Fulfilled());
                    return;
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    WalkChildren(variableDeclaration);
                    Validate(variableDeclaration.Type!);
                    break;
            }

            WalkChildren(syntax);
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
                //case PointerType _:
                //    // can't contain a reference type
                //    break;
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
                    //case PointerType _:
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
