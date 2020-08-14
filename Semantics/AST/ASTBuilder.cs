using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;
using Package = Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree.Package;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST
{
    // ReSharper disable once UnusedMember.Global
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes",
        Justification = "In Progress")]
    internal class ASTBuilder
    {
        public Package BuildPackage(FixedList<IEntityDeclarationSyntax> entities)
        {
            var astEntities = new List<INonMemberDeclaration>();
            foreach (var entity in entities.OfType<INonMemberEntityDeclarationSyntax>())
            {
                astEntities.Add(BuildNonMemberDeclaration(entity));
            }

            return new Package();
        }

        private INonMemberDeclaration BuildNonMemberDeclaration(INonMemberEntityDeclarationSyntax entity)
        {
            return entity switch
            {
                IClassDeclarationSyntax _ => throw new NotImplementedException(),
                IFunctionDeclarationSyntax syn => BuildFunction(syn),
                _ => throw ExhaustiveMatch.Failed(entity)
            };
        }

        private INonMemberDeclaration BuildFunction(IFunctionDeclarationSyntax syn)
        {
            throw new NotImplementedException();
            //return new FunctionDeclaration();
        }
    }
}
