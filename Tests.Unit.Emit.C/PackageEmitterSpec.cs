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
    public class PackageEmitterSpec
    {
        [Fact]
        [Category("Emitter")]
        public void PreambleSetsUpEachSection()
        {
            var code = new Code();
            NewPackageEmitter().EmitPreamble(code);
            const string expected =
@"#include ""RuntimeLibrary.h""

// Type ID Declarations
enum Type_ID
{
    ₐnever·ₐTypeID = 0,

// Type Declarations

// Function Declarations

// Struct Declarations

// Global Definitions

// Definitions
";
            Assert.Equal(expected.NormalizeLineEndings(CCodeBuilder.LineTerminator), code.ToString());
            Assert.Equal(1, code.TypeIdDeclaration.CurrentIndentDepth);
        }

        [Fact]
        public void PostambleClosesTypeIdEnum()
        {
            var code = new Code();
            code.TypeIdDeclaration.BeginBlock();
            NewPackageEmitter().EmitPostamble(code);
            const string expected =
@"{
};
typedef enum Type_ID Type_ID;
";
            Assert.Equal(expected.NormalizeLineEndings(CCodeBuilder.LineTerminator), code.TypeIdDeclaration.Code);
        }

        [Fact]
        public void EmitsEntryPointAdapterForNoArgVoidReturnMain()
        {
            var main = new FunctionDeclaration("".ToFakeCodeFile(),
                new SimpleName("main"),
                new FakeAdamantType(),
                Enumerable.Empty<Parameter>(),
                ObjectType.Void,
                new ControlFlowGraph());

            var code = new Code();
            NewPackageEmitter().EmitEntryPointAdapter(main, code);
            const string expected =
@"// Entry Point Adapter
int32_t main(const int argc, char const * const * const argv)
{
    ᵢmain´0();
    return 0;
}
";
            Assert.Equal(expected.NormalizeLineEndings(CCodeBuilder.LineTerminator), code.Definitions.Code);
        }

        [NotNull]
        private static PackageEmitter NewPackageEmitter()
        {
            var nameMangler = new NameMangler();
            var declarationEmitter = new FakeEmitter<Declaration>();
            return new PackageEmitter(nameMangler, declarationEmitter);
        }
    }
}
