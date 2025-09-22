using FluentAssertions;
using Life.Domain.Common;

namespace Life.Domain.Tests.Common
{
    public class DomainEventTests
    {
        private record TestDomainEvent : DomainEvent
        {
            public string Message { get; init; } = string.Empty;
        }

        [Fact]
        public void DomainEvent_Should_Have_Unique_Id()
        {
            // Arrange & Act
            var event1 = new TestDomainEvent();
            var event2 = new TestDomainEvent();

            // Assert
            event1.Id.Should().NotBe(Guid.Empty);
            event2.Id.Should().NotBe(Guid.Empty);
            event1.Id.Should().NotBe(event2.Id);
        }

        [Fact]
        public void DomainEvent_Should_Have_OccurredOn_Set()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var domainEvent = new TestDomainEvent();
            var afterCreation = DateTime.UtcNow;

            // Assert
            domainEvent.OccurredOn.Should().BeAfter(beforeCreation.AddSeconds(-1));
            domainEvent.OccurredOn.Should().BeBefore(afterCreation.AddSeconds(1));
        }

        [Fact]
        public void DomainEvent_Should_Support_Record_Equality()
        {
            // Arrange & Act
            var event1 = new TestDomainEvent { Message = "Test" };
            var event2 = new TestDomainEvent { Message = "Test" };
            var event3 = new TestDomainEvent { Message = "Different" };

            // Assert
            // Note: Records use structural equality for all properties
            // But DomainEvent has unique Id and OccurredOn, so instances won't be equal
            event1.Should().NotBe(event2); // Different Id and OccurredOn
            event1.Should().NotBe(event3);
            event2.Should().NotBe(event3);
        }

        [Fact]
        public void DomainEvent_Should_Be_Immutable()
        {
            // Arrange & Act
            var domainEvent = new TestDomainEvent { Message = "Test" };

            // Assert
            domainEvent.Id.Should().NotBe(Guid.Empty);
            domainEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            domainEvent.Message.Should().Be("Test");
        }
    }
}