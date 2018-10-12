using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C
{
    [UnitTest]
    [Category("Emit")]
    public class DeclarationEmitterSpec
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Empty_function(int parameterCount)
        {
            var parameters = Enumerable.Repeat(0, parameterCount)
                .Select(_ => new FakeParameter()).ToList();
            var returnType = new FakeDataType();
            var function = new FunctionDeclaration("".ToFakeCodeFile(),
                new QualifiedName(new SimpleName("func")),
                parameters,
                returnType);

            var code = new Code();
            NewDeclarationEmitter().Emit(function, code);

            var parametersString = string.Join(", ", parameters);
            var declarations = $"{returnType} ᵢfunc´{parameterCount}({parametersString});\n";
            var definitions =
$@"{returnType} ᵢfunc´{parameterCount}({parametersString})
{{
}}
".NormalizeLineEndings(CCodeBuilder.LineTerminator);

            Assert.Equal("", code.Includes.Code);
            Assert.Equal("", code.TypeIdDeclaration.Code);
            Assert.Equal("", code.TypeDeclarations.Code);
            Assert.Equal(declarations, code.FunctionDeclarations.Code);
            Assert.Equal("", code.ClassDeclarations.Code);
            Assert.Equal("", code.GlobalDefinitions.Code);
            Assert.Equal(definitions, code.Definitions.Code);
        }

        [NotNull]
        private static DeclarationEmitter NewDeclarationEmitter()
        {
            var nameMangler = new NameMangler();
            var parameterConverter = new FakeConverter<Parameter>();
            var typeConvert = new FakeConverter<DataType>();
            return new DeclarationEmitter(nameMangler, parameterConverter, typeConvert);
        }
    }
}
