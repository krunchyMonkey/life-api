using FluentAssertions;
using Life.Domain.Aggregates;
using Life.Domain.Services;

namespace Life.Domain.Tests
{
    public class FinalDetectorTests
    {
        [Fact]
        public void Detects_Stable()
        {
            // Arrange
            var detector = new FinalDetector(new BoardHasher());
            var start = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });

            // Act
            var res = detector.Detect(start, b => new Life.Domain.Services.Game().Next(b), 1000, TimeSpan.FromMilliseconds(200));

            // Assert
            res.Stable.Should().BeTrue();
            res.Cyclic.Should().BeFalse();
        }

        [Fact]
        public void Detects_Cycle()
        {
            // Arrange
            var detector = new FinalDetector(new BoardHasher());
            var start = new Board(5, 5, new[] { (2, 1), (2, 2), (2, 3) });

            // Act
            var res = detector.Detect(start, b => new Life.Domain.Services.Game().Next(b), 1000, TimeSpan.FromMilliseconds(200));

            // Assert
            res.Cyclic.Should().BeTrue();
        }
    }
}
