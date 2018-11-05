using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
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
            var functionType = new FakeAdamantType();
            var parameters = Enumerable.Repeat(0, parameterCount)
                .Select(_ => new FakeParameter()).ToList();
            var returnType = new FakeAdamantType();
            var function = new FunctionDeclaration("".ToFakeCodeFile(),
                GlobalNamespaceName.Instance.Qualify("func"),
                functionType,
                parameters,
                returnType,
                new ControlFlowGraph());

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
            Assert.Equal("", code.StructDeclarations.Code);
            Assert.Equal("", code.GlobalDefinitions.Code);
            Assert.Equal(definitions, code.Definitions.Code);
        }

        [Fact]
        public void Type()
        {
            var declaredType = new FakeAdamantType();
            var typeName = GlobalNamespaceName.Instance.Qualify("My_Type");
            var type = new TypeDeclaration("".ToFakeCodeFile(), typeName, declaredType);

            var code = new Code();
            NewDeclarationEmitter().Emit(type, code);

            var declarations =
@"typedef struct ᵢMy_Type´0·ₐSelf ᵢMy_Type´0·ₐSelf;
typedef struct ᵢMy_Type´0·ₐV_Table ᵢMy_Type´0·ₐV_Table;
typedef struct { ᵢMy_Type´0·ₐV_Table const*_Nonnull restrict ₐvtable; ᵢMy_Type´0·ₐSelf const*_Nonnull restrict ₐself; } ᵢMy_Type´0;
typedef struct { ᵢMy_Type´0·ₐV_Table const*_Nonnull restrict ₐvtable; ᵢMy_Type´0·ₐSelf *_Nonnull restrict ₐself; } mut˽ᵢMy_Type´0;
"
.NormalizeLineEndings(CCodeBuilder.LineTerminator);
            var structDeclarations =
@"struct ᵢMy_Type´0·ₐSelf
{
};
".NormalizeLineEndings(CCodeBuilder.LineTerminator);
            Assert.Equal("", code.Includes.Code);
            Assert.Equal("", code.TypeIdDeclaration.Code);
            Assert.Equal(declarations, code.TypeDeclarations.Code);
            Assert.Equal("", code.FunctionDeclarations.Code);
            Assert.Equal(structDeclarations, code.StructDeclarations.Code);
            Assert.Equal("", code.GlobalDefinitions.Code);
            Assert.Equal("", code.Definitions.Code);
        }

        [NotNull]
        private static DeclarationEmitter NewDeclarationEmitter()
        {
            var nameMangler = new NameMangler();
            var parameterConverter = new FakeConverter<Parameter>();
            var typeConvert = new FakeConverter<KnownType>();
            var controlFlowEmitter = new FakeEmitter<ControlFlowGraph>();
            return new DeclarationEmitter(nameMangler, parameterConverter, typeConvert, controlFlowEmitter);
        }
    }
}
