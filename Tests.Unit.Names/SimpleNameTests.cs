using Adamant.Tools.Compiler.Bootstrap.Names;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Names
{
    [Trait("Category", "Names")]
    public class SimpleNameTests
    {
        [Fact]
        public void Unqualified_name_is_same()
        {
            var name = new SimpleName("Something");

            var unqualifiedName = name.UnqualifiedName;

            Assert.Same(name, unqualifiedName);
        }

        [Fact]
        public void New_name_has_correct_properties()
        {
            var name = new SimpleName("Something");

            Assert.Equal("Something", name.Text);
            Assert.Null(name.DeclarationNumber);
            Assert.False(name.IsSpecial);
        }

        [Fact]
        public void Variable_name_has_text_and_declaration_number()
        {
            var variable = SimpleName.Variable("xdf", 42);

            Assert.Equal("xdf", variable.Text);
            Assert.Equal(42, variable.DeclarationNumber);
            Assert.False(variable.IsSpecial);
        }

        [Fact]
        public void Special_name_has_text_and_is_special()
        {
            var special = SimpleName.Special("pqr");

            Assert.Equal("pqr", special.Text);
            Assert.Null(special.DeclarationNumber);
            Assert.True(special.IsSpecial);
        }

        [Fact]
        public void Has_one_segment_itself()
        {
            var name = new SimpleName("Something");

            Assert.Equal(new[] { name }, name.Segments);
        }

        [Fact]
        public void Not_nested_in_other_names()
        {
            var name = new SimpleName("Something");

            Assert.Empty(name.NestedInNames());
        }

        [Fact]
        public void Namespace_names_is_itself()
        {
            var name = new SimpleName("Something");

            Assert.Equal(new[] { name }, name.NamespaceNames());
        }

        [Fact]
        public void Remove_declaration_number_of_name_without_number_returns_same_name()
        {
            var name = new SimpleName("Something");

            Assert.Same(name, name.WithoutDeclarationNumber());
        }

        [Fact]
        public void Remove_declaration_number_is_equal_except_without_number()
        {
            var variable = SimpleName.Variable("xdf", 42);

            var withoutDeclarationNumber = variable.WithoutDeclarationNumber();

            Assert.Equal(variable.Text, withoutDeclarationNumber.Text);
            Assert.Equal(variable.IsSpecial, withoutDeclarationNumber.IsSpecial);
            Assert.Null(withoutDeclarationNumber.DeclarationNumber);
        }

        [Fact]
        public void Names_with_same_text_are_equal()
        {
            var name1 = new SimpleName("foo");
            var name2 = new SimpleName("foo");

            Assert.Equal(name1, name2);
        }

        [Fact]
        public void Variables_with_different_declaration_numbers_are_not_equal()
        {
            var var1 = SimpleName.Variable("foo", 1);
            var var2 = SimpleName.Variable("foo", 2);

            Assert.NotEqual(var1, var2);
        }

        [Fact]
        public void Does_not_have_qualifier()
        {
            var name = new SimpleName("foo");

            var hasQualifier = name.HasQualifier(MaybeQualifiedName.From("bar"));

            Assert.False(hasQualifier);
        }

        [Fact]
        public void Is_not_nested_in_other_names()
        {
            var name = new SimpleName("foo");

            var hasQualifier = name.IsNestedIn(MaybeQualifiedName.From("bar"));

            Assert.False(hasQualifier);
        }
    }
}
