using FluentAssertions;
using Life.Domain.Events;

namespace Life.Domain.Tests.Events
{
    public class BoardEventsTests
    {
        [Fact]
        public void BoardGenerationAdvanced_Should_Have_Correct_Properties()
        {
            // Arrange
            var originalBoardId = Guid.NewGuid();
            var newBoardId = Guid.NewGuid();

            // Act
            var boardEvent = new BoardGenerationAdvanced(originalBoardId, newBoardId);

            // Assert
            boardEvent.OriginalBoardId.Should().Be(originalBoardId);
            boardEvent.NewBoardId.Should().Be(newBoardId);
            boardEvent.Id.Should().NotBe(Guid.Empty);
            boardEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void BoardMultipleGenerationsAdvanced_Should_Have_Correct_Properties()
        {
            // Arrange
            var originalBoardId = Guid.NewGuid();
            var finalBoardId = Guid.NewGuid();
            var generationCount = 5L;

            // Act
            var boardEvent = new BoardMultipleGenerationsAdvanced(originalBoardId, finalBoardId, generationCount);

            // Assert
            boardEvent.OriginalBoardId.Should().Be(originalBoardId);
            boardEvent.FinalBoardId.Should().Be(finalBoardId);
            boardEvent.GenerationCount.Should().Be(generationCount);
            boardEvent.Id.Should().NotBe(Guid.Empty);
            boardEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void BoardFinalStateDetected_Should_Have_Correct_Properties_For_Stable_State()
        {
            // Arrange
            var originalBoardId = Guid.NewGuid();
            var finalBoardId = Guid.NewGuid();

            // Act
            var boardEvent = new BoardFinalStateDetected(originalBoardId, finalBoardId, false, true, null);

            // Assert
            boardEvent.OriginalBoardId.Should().Be(originalBoardId);
            boardEvent.FinalBoardId.Should().Be(finalBoardId);
            boardEvent.IsCyclic.Should().BeFalse();
            boardEvent.IsStable.Should().BeTrue();
            boardEvent.CycleLength.Should().BeNull();
            boardEvent.Id.Should().NotBe(Guid.Empty);
            boardEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void BoardFinalStateDetected_Should_Have_Correct_Properties_For_Cyclic_State()
        {
            // Arrange
            var originalBoardId = Guid.NewGuid();
            var finalBoardId = Guid.NewGuid();
            var cycleLength = 3;

            // Act
            var boardEvent = new BoardFinalStateDetected(originalBoardId, finalBoardId, true, false, cycleLength);

            // Assert
            boardEvent.OriginalBoardId.Should().Be(originalBoardId);
            boardEvent.FinalBoardId.Should().Be(finalBoardId);
            boardEvent.IsCyclic.Should().BeTrue();
            boardEvent.IsStable.Should().BeFalse();
            boardEvent.CycleLength.Should().Be(cycleLength);
            boardEvent.Id.Should().NotBe(Guid.Empty);
            boardEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void BoardEvents_Should_Support_Record_Equality()
        {
            // Arrange
            var originalId = Guid.NewGuid();
            var newId = Guid.NewGuid();

            // Act
            var event1 = new BoardGenerationAdvanced(originalId, newId);
            var event2 = new BoardGenerationAdvanced(originalId, newId);
            var event3 = new BoardGenerationAdvanced(Guid.NewGuid(), newId);

            // Assert
            // Records will not be equal due to unique Id and OccurredOn in base DomainEvent
            event1.Should().NotBe(event2);
            event1.Should().NotBe(event3);
            event2.Should().NotBe(event3);
            
            // But the custom properties should match
            event1.OriginalBoardId.Should().Be(event2.OriginalBoardId);
            event1.NewBoardId.Should().Be(event2.NewBoardId);
        }
    }
}