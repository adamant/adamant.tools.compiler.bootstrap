using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class NameAttribute : SemanticAttribute
    {
        public const string Key = "Name";
        public override string AttributeKey => Key;

        public NameAttribute(SemanticAttributes attributes)
            : base(attributes)
        {
        }

        public PackageName this[PackageSyntax s] => Get<PackageName>(s);
        public GlobalNamespaceName this[CompilationUnitSyntax s] => Get<GlobalNamespaceName>(s);
        public FunctionName this[FunctionDeclarationSyntax s] => Get<FunctionName>(s);
        public VariableName this[ParameterSyntax s] => Get<VariableName>(s);
        public VariableName this[IdentifierNameSyntax s] => Get<VariableName>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Name Get(SyntaxBranchNode syntax)
        {
            return Attributes.Get(syntax, Key, Compute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TName Get<TName>(SyntaxBranchNode syntax)
            where TName : Name
        {
            return (TName)Attributes.Get(syntax, Key, Compute);
        }

        private Name Compute(SyntaxBranchNode syntax)
        {
            switch (syntax)
            {
                case PackageSyntax package:
                    // TODO get actual package name
                    return new PackageName("default");

                case CompilationUnitSyntax compilationUnit:
                    return GetGlobalNamespace(Parent[compilationUnit]);

                case FunctionDeclarationSyntax functionDeclaration:
                    var parentName = Name[Parent[functionDeclaration]];
                    return new FunctionName(parentName, functionDeclaration.Name.Value, functionDeclaration.ParameterList.Parameters.Count());

                case ParameterSyntax parameter:
                case IdentifierNameSyntax identifierName:
                    throw new NotImplementedException();

                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GlobalNamespaceName GetGlobalNamespace(PackageSyntax s) => Attributes.Get(s, "GlobalNamespaceName", ComputeGlobalNamespaceName);

        private GlobalNamespaceName ComputeGlobalNamespaceName(PackageSyntax package)
        {
            return new GlobalNamespaceName(Name[package]);
        }
    }
}
