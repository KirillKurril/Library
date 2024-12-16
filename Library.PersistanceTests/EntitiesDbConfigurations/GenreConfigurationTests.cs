using FluentAssertions;
using Library.Domain.Entities;
using Library.Persistance.Contexts;
using Library.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.IntegrationTests.EntitiesDbConfigurations
{
    public class GenreConfigurationTests : TestBase
    {
        [Fact]
        public void GenreConfiguration_PrimaryKey_ShouldBeConfiguredCorrectly()
        {
            using var context = CreateContext();

            var entityType = context.Model.FindEntityType(typeof(Genre));

            var primaryKey = entityType.FindPrimaryKey();
            primaryKey.Should().NotBeNull();
            primaryKey.Properties.Single().Name.Should().Be(nameof(Genre.Id));
        }

        [Fact]
        public void GenreConfiguration_RequiredProperties_ShouldBeConfiguredCorrectly()
        {
            using var context = CreateContext();

            var entityType = context.Model.FindEntityType(typeof(Genre));

            var nameProperty = entityType.FindProperty(nameof(Genre.Name));
            nameProperty.Should().NotBeNull();
            nameProperty.IsNullable.Should().BeFalse();
            nameProperty.GetMaxLength().Should().Be(100);
        }

        [Fact]
        public async Task GenreConfiguration_DefaultId_ShouldBeGenerated()
        {
            using var context = CreateContext();

            var genre = new Genre { Name = "Test Genre" };

            context.Genres.Add(genre);
            await context.SaveChangesAsync();

            genre.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task GenreConfiguration_UniqueConstraint_ShouldPreventDuplicateNames()
        {
            using var context = CreateContext();

            var genre1 = new Genre { Name = "Test Genre" };
            context.Genres.Add(genre1);
            await context.SaveChangesAsync();

            var genre2 = new Genre { Name = "Test Genre" };
            context.Genres.Add(genre2);

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public async Task GenreConfiguration_NameMaxLength_ShouldEnforceLimit()
        {
            using var context = CreateContext();

            var longName = new string('A', 101); 
            var genre = new Genre { Name = longName };

            context.Genres.Add(genre);
            
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }
    }
}
