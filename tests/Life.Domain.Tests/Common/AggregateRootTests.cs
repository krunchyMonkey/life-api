using FluentAssertions;
using Life.Domain.Common;

namespace Life.Domain.Tests.Common
{
    public class AggregateRootTests
    {
        private class TestAggregateRoot : AggregateRoot<Guid>
        {
            public TestAggregateRoot(Guid id) : base(id) { }
            public TestAggregateRoot() : base() { }
            
            public string Name { get; set; } = string.Empty;
            
            public void TriggerEvent(DomainEvent domainEvent)
            {
                AddDomainEvent(domainEvent);
            }
        }

        private record TestDomainEvent : DomainEvent { }

        [Fact]
        public void AggregateRoot_Should_Initialize_With_Empty_Domain_Events()
        {
            // Arrange & Act
            var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());

            // Assert
            aggregateRoot.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public void AggregateRoot_Should_Add_Domain_Events()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            var domainEvent = new TestDomainEvent();

            // Act
            aggregateRoot.TriggerEvent(domainEvent);

            // Assert
            aggregateRoot.DomainEvents.Should().HaveCount(1);
            aggregateRoot.DomainEvents.Should().Contain(domainEvent);
        }

        [Fact]
        public void AggregateRoot_Should_Add_Multiple_Domain_Events()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            var event1 = new TestDomainEvent();
            var event2 = new TestDomainEvent();

            // Act
            aggregateRoot.TriggerEvent(event1);
            aggregateRoot.TriggerEvent(event2);

            // Assert
            aggregateRoot.DomainEvents.Should().HaveCount(2);
            aggregateRoot.DomainEvents.Should().Contain(event1);
            aggregateRoot.DomainEvents.Should().Contain(event2);
        }

        [Fact]
        public void AggregateRoot_Should_Clear_Domain_Events()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.TriggerEvent(new TestDomainEvent());
            aggregateRoot.TriggerEvent(new TestDomainEvent());

            // Act
            aggregateRoot.ClearDomainEvents();

            // Assert
            aggregateRoot.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public void DomainEvents_Should_Be_ReadOnly_Collection()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.TriggerEvent(new TestDomainEvent());

            // Act & Assert
            aggregateRoot.DomainEvents.Should().BeAssignableTo<IReadOnlyCollection<DomainEvent>>();
        }

        [Fact]
        public void AggregateRoot_Should_Inherit_Entity_Behavior()
        {
            // Arrange
            var id = Guid.NewGuid();
            var aggregateRoot1 = new TestAggregateRoot(id) { Name = "Test1" };
            var aggregateRoot2 = new TestAggregateRoot(id) { Name = "Test2" };

            // Act & Assert
            aggregateRoot1.Should().Be(aggregateRoot2);
            aggregateRoot1.Id.Should().Be(id);
        }
    }
}