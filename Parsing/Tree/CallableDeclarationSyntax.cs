using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class CallableDeclarationSyntax : DeclarationSyntax, ICallableDeclarationSyntax
    {
        public IAccessModifierToken? AccessModifier { get; }
        public Name FullName { get; }
        private DataType? selfParameterType;
        [DisallowNull]
        public DataType? SelfParameterType
        {
            get => selfParameterType;
            set
            {
                if (selfParameterType != null)
                    throw new InvalidOperationException("Can't set SelfParameterType repeatedly");
                selfParameterType = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public FixedList<IParameterSyntax> Parameters { get; }
        IEnumerable<IBindingMetadata> IFunctionMetadata.Parameters => Parameters;
        public TypePromise ReturnType { get; } = new TypePromise();
        DataType IFunctionMetadata.ReturnType => ReturnType.Fulfilled();
        public FixedList<IReachabilityAnnotationSyntax> ReachabilityAnnotations { get; }

        public MetadataSet ChildMetadata { get; protected set; }

        protected CallableDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            Name fullName,
            TextSpan nameSpan,
            IEnumerable<IParameterSyntax> parameters,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            MetadataSet childMetadata)
            : base(span, file, nameSpan)
        {
            AccessModifier = accessModifier;
            FullName = fullName;
            Parameters = parameters.ToFixedList();
            ReachabilityAnnotations = reachabilityAnnotations;
            ChildMetadata = childMetadata;
        }

        protected static MetadataSet GetChildMetadata(
            ISelfParameterSyntax? selfParameter,
            IEnumerable<IParameterSyntax> parameters,
            IBodySyntax? body)
        {
            if (!(selfParameter is null))
                parameters = parameters.Prepend(selfParameter);

            var variableDeclarations = body?.GetAllVariableDeclarations()
                                       ?? Enumerable.Empty<IBindingMetadata>();
            var childSymbols = parameters.Concat(variableDeclarations);
            return new MetadataSet(childSymbols);
        }
    }
}
