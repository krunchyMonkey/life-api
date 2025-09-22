using FluentAssertions;
using Life.Domain.Common;

namespace Life.Domain.Tests.Common
{
    public class EntityTests
    {
        private class TestEntity : Entity<int>
        {
            public TestEntity(int id) : base(id) { }
            public TestEntity() : base() { }
            
            public string Name { get; set; } = string.Empty;
        }

        [Fact]
        public void Entity_Should_Have_Id_When_Created_With_Id()
        {
            // Arrange & Act
            var entity = new TestEntity(123);

            // Assert
            entity.Id.Should().Be(123);
        }

        [Fact]
        public void Entity_Should_Have_Default_Id_When_Created_Without_Id()
        {
            // Arrange & Act
            var entity = new TestEntity();

            // Assert
            entity.Id.Should().Be(default(int));
        }

        [Fact]
        public void Entities_With_Same_Id_Should_Be_Equal()
        {
            // Arrange
            var entity1 = new TestEntity(1) { Name = "Test1" };
            var entity2 = new TestEntity(1) { Name = "Test2" };

            // Act & Assert
            entity1.Should().Be(entity2);
            entity1.GetHashCode().Should().Be(entity2.GetHashCode());
            (entity1 == entity2).Should().BeTrue();
            (entity1 != entity2).Should().BeFalse();
        }

        [Fact]
        public void Entities_With_Different_Ids_Should_Not_Be_Equal()
        {
            // Arrange
            var entity1 = new TestEntity(1);
            var entity2 = new TestEntity(2);

            // Act & Assert
            entity1.Should().NotBe(entity2);
            entity1.GetHashCode().Should().NotBe(entity2.GetHashCode());
            (entity1 == entity2).Should().BeFalse();
            (entity1 != entity2).Should().BeTrue();
        }

        [Fact]
        public void Entity_Should_Not_Equal_Null()
        {
            // Arrange
            var entity = new TestEntity(1);

            // Act & Assert
            entity.Should().NotBeNull();
            entity.Equals(null).Should().BeFalse();
            (entity == null).Should().BeFalse();
            (entity != null).Should().BeTrue();
        }

        [Fact]
        public void Entity_Should_Not_Equal_Different_Type()
        {
            // Arrange
            var entity = new TestEntity(1);
            var otherObject = "string";

            // Act & Assert
            entity.Equals(otherObject).Should().BeFalse();
        }

        [Fact]
        public void Entity_Should_Return_Consistent_HashCode()
        {
            // Arrange
            var entity = new TestEntity(123);

            // Act
            var hashCode1 = entity.GetHashCode();
            var hashCode2 = entity.GetHashCode();

            // Assert
            hashCode1.Should().Be(hashCode2);
        }
    }
}