using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations
{
    public class ClassDeclaration : MemberDeclaration
    {
        public new ClassDeclarationSyntax Syntax { get; }
        public AccessLevel Access { get; }
        public bool IsMutable { get; }
        public string Name { get; }
        public ObjectType Type { get; }
        public IReadOnlyList<MemberDeclaration> Members { get; }

        public ClassDeclaration(
            ClassDeclarationSyntax syntax,
            IEnumerable<DiagnosticInfo> diagnostics,
            AccessLevel access,
            ObjectType type,
            IEnumerable<MemberDeclaration> members)
            : base(diagnostics)
        {
            Syntax = syntax;
            Access = access;
            IsMutable = type.IsMutable;
            Name = type.Name.EntityName;
            Type = type;
            Members = members.ToList().AsReadOnly();
        }

        protected override SyntaxNode GetSyntax() => Syntax;
    }
}
