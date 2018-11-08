using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses.Builders;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics
{
    [UnitTest]
    [Category("Analyze")]
    public class DeclarationAnalysisBuilderSpec
    {
        [Fact]
        public void Ignores_incomplete_declarations()
        {
            var incompleteDeclaration = FakeSyntax.IncompleteDeclaration();

            var analysis = Build(incompleteDeclaration);

            Assert.Null(analysis);
        }

        [Fact]
        public void Function_without_namespace()
        {
            var functionDeclaration = FakeSyntax.FunctionDeclaration("function_name");

            var analysis = Build(functionDeclaration);

            var f = Assert.IsType<FunctionDeclarationAnalysis>(analysis);
            Assert.Equal("function_name", f.Name.ToString());
        }

        [Fact]
        public void Function_with_namespace()
        {
            var functionDeclaration = FakeSyntax.FunctionDeclaration("function_name");
            var @namespace = Name.From("myNamespace", "name");

            var analysis = Build(functionDeclaration, @namespace);

            var f = Assert.IsType<FunctionDeclarationAnalysis>(analysis);
            Assert.Equal("myNamespace.name.function_name", f.Name.ToString());
        }

        [Fact]
        public void Enum_struct()
        {
            var enumDeclaration = FakeSyntax.EnumStructDeclaration("My_Struct");

            var analysis = Build(enumDeclaration);

            var t = Assert.IsType<TypeDeclarationAnalysis>(analysis);
            Assert.Equal("My_Struct", t.Name.ToString());
        }

        private static MemberDeclarationAnalysis Build(
            [NotNull] DeclarationSyntax declaration,
            [CanBeNull] RootName @namespace = null)
        {
            var nameBuilder = new NameBuilder();
            var expressionBuilder = new FakeExpressionAnalysisBuilder();
            var statementBuilder = new FakeStatementAnalysisBuilder();
            var declarationBuilder = new DeclarationAnalysisBuilder(expressionBuilder, statementBuilder, nameBuilder);
            var context = new AnalysisContext("".ToFakeCodeFile(), new FakeLexicalScope());
            return declarationBuilder.BuildDeclaration(context, @namespace ?? GlobalNamespaceName.Instance, declaration);
        }
    }
}
