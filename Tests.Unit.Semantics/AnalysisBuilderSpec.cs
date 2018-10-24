using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics
{
    [UnitTest]
    [Category("Analyze")]
    public class AnalysisBuilderSpec
    {
        [Fact]
        public void Ignores_incomplete_declarations()
        {
            var incompleteDeclaration = FakeSyntax.IncompleteDeclaration();

            var analysis = PrepareForAnalysis(incompleteDeclaration);

            Assert.Null(analysis);
        }

        //[Fact]
        //public void Function_without_namespace()
        //{
        //    var functionDeclaration = FakeSyntax.FunctionDeclaration("function_name");
        //    var compilationUnitSyntax = FakeSyntax.CompilationUnit(functionDeclaration);
        //    var packageSyntax = FakeSyntax.Package(compilationUnitSyntax);

        //    var (scopes, analyses) = PrepareForAnalysis(packageSyntax);

        //    Assert.Collection(analyses, a =>
        //    {
        //        var f = Assert.IsType<FunctionDeclarationAnalysis>(a);
        //        Assert.Equal("function_name", f.QualifiedName.ToString());
        //    });
        //}

        //[Fact]
        //public void Function_with_namespace()
        //{
        //    var functionDeclaration = FakeSyntax.FunctionDeclaration("function_name");
        //    var @namespace = FakeSyntax.Name("myNamespace.name");
        //    var compilationUnitSyntax = FakeSyntax.CompilationUnit(@namespace, functionDeclaration);
        //    var packageSyntax = FakeSyntax.Package(compilationUnitSyntax);

        //    var (scopes, analyses) = PrepareForAnalysis(packageSyntax);

        //    Assert.Collection(analyses, a =>
        //    {
        //        var f = Assert.IsType<FunctionDeclarationAnalysis>(a);
        //        Assert.Equal("myNamespace.name.function_name", f.QualifiedName.ToString());
        //    });
        //}

        //[Fact]
        //public void Enum_struct()
        //{
        //    var functionDeclaration = FakeSyntax.EnumStructDeclaration("My_Struct");
        //    var compilationUnitSyntax = FakeSyntax.CompilationUnit(functionDeclaration);
        //    var packageSyntax = FakeSyntax.Package(compilationUnitSyntax);

        //    var (scopes, analyses) = PrepareForAnalysis(packageSyntax);

        //    Assert.Collection(analyses, a =>
        //    {
        //        var t = Assert.IsType<TypeDeclarationAnalysis>(a);
        //        Assert.Equal("My_Struct", t.QualifiedName.ToString());
        //    });
        //}

        private static MemberDeclarationAnalysis PrepareForAnalysis(
            [NotNull] DeclarationSyntax declaration)
        {
            var nameBuilder = new NameBuilder();
            StatementAnalysisBuilder statementBuilder = null;
            var expressionBuilder = new ExpressionAnalysisBuilder(() => statementBuilder);
            statementBuilder = new StatementAnalysisBuilder(expressionBuilder);
            var declarationBuilder = new DeclarationAnalysisBuilder(nameBuilder, expressionBuilder, statementBuilder);
            var context = new AnalysisContext("".ToFakeCodeFile(), new FakeLexicalScope());
            return declarationBuilder.Build(context, null, declaration);
        }
    }
}
