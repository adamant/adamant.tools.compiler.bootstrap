using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class TypeAttribute : SemanticAttribute
    {
        public const string Key = "Type";
        public override string AttributeKey => Key;

        public TypeAttribute(SemanticAttributes attributes)
        : base(attributes)
        {
        }

        public DataType this[ParameterSyntax syntax] => Get(syntax);
        public DataType this[TypeSyntax syntax] => Get(syntax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataType Get(SyntaxBranchNode syntax)
        {
            return Attributes.Get(syntax, Key, Compute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TDataType Get<TDataType>(SyntaxBranchNode syntax)
            where TDataType : DataType
        {
            return (TDataType)Attributes.Get(syntax, Key, Compute);
        }

        private DataType Compute(SyntaxBranchNode syntax)
        {
            throw new System.NotImplementedException();
        }
    }
}
