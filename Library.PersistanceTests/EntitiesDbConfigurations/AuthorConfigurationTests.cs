using FluentAssertions;
using Library.Domain.Entities;

namespace Library.IntegrationTests.EntitiesBdConfigurations
{
    public class AuthorConfigurationTests : TestBase
    {
        [Fact]
        public void AuthorConfiguration_PrimaryKey_ShouldBeConfiguredCorrectly()
        {

            using var context = CreateContext();

            var entityType = context.Model.FindEntityType(typeof(Author));

            
            var primaryKey = entityType.FindPrimaryKey();
            primaryKey.Should().NotBeNull();
            primaryKey.Properties.Single().Name.Should().Be(nameof(Author.Id));
        }

        [Fact]
        public void AuthorConfiguration_RequiredProperties_ShouldBeConfiguredCorrectly()
        {

            using var context = CreateContext();
            var entityType = context.Model.FindEntityType(typeof(Author));

            
            var nameProperty = entityType.FindProperty(nameof(Author.Name));
            nameProperty.Should().NotBeNull();
            nameProperty.IsNullable.Should().BeFalse();
            nameProperty.GetMaxLength().Should().Be(100);
        }

        [Fact]
        public async Task AuthorConfiguration_DefaultId_ShouldBeGenerated()
        {

            using var context = CreateContext();
            var author = new Author { Name = "Test Author" };


            context.Authors.Add(author);
            await context.SaveChangesAsync();

            
            author.Id.Should().NotBe(Guid.Empty);
        }
    }
}
