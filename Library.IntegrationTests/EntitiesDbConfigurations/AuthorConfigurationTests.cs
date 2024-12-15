using FluentAssertions;
using Library.Domain.Entities;
using Library.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Library.IntegrationTests.EntitiesBdConfigurations
{
    public class AuthorConfigurationTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public AuthorConfigurationTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void AuthorConfiguration_PrimaryKey_ShouldBeConfiguredCorrectly()
        {
            
            using var context = new AppDbContext(_options);
            var entityType = context.Model.FindEntityType(typeof(Author));

            
            var primaryKey = entityType.FindPrimaryKey();
            primaryKey.Should().NotBeNull();
            primaryKey.Properties.Single().Name.Should().Be(nameof(Author.Id));
        }

        [Fact]
        public void AuthorConfiguration_RequiredProperties_ShouldBeConfiguredCorrectly()
        {
            
            using var context = new AppDbContext(_options);
            var entityType = context.Model.FindEntityType(typeof(Author));

            
            var nameProperty = entityType.FindProperty(nameof(Author.Name));
            nameProperty.Should().NotBeNull();
            nameProperty.IsNullable.Should().BeFalse();
            nameProperty.GetMaxLength().Should().Be(100);
        }

        [Fact]
        public async Task AuthorConfiguration_DefaultId_ShouldBeGenerated()
        {
            
            using var context = new AppDbContext(_options);
            var author = new Author { Name = "Test Author" };


            context.Authors.Add(author);
            await context.SaveChangesAsync();

            
            author.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task AuthorConfiguration_UniqueConstraint_ShouldPreventDuplicateNames()
        {
            
            using var context = new AppDbContext(_options);
            var author1 = new Author { Name = "Test Author" };
            var author2 = new Author { Name = "Test Author" };


            context.Authors.Add(author1);
            await context.SaveChangesAsync();

            context.Authors.Add(author2);
            
            
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }
    }
}
