using Life.Domain.Models;
using Life.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Life.Domain.Tests
{
    public class GameTests
    {
        [Fact]
        public void Blinker_Oscillates()
        {
            // Arrange
            var game = new Game();
            var start = new Board(5, 5, new[] { (2, 1), (2, 2), (2, 3) });

            // Act
            var next = game.Next(start);

            // Assert
            next.Alive().OrderBy(c => c.y).ThenBy(c => c.x).Should().BeEquivalentTo(new[] { (1, 2), (2, 2), (3, 2) });
            var back = game.Next(next);
            back.Alive().OrderBy(c => c.y).ThenBy(c => c.x).Should().BeEquivalentTo(new[] { (2, 1), (2, 2), (2, 3) });
        }

        [Fact]
        public void Advance_N_Generations()
        {
            // Arrange
            var game = new Game();

            // Act
            var start = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });
            var adv = game.Advance(start, 10);

            // Assert
            adv.Alive().OrderBy(c => c.y).ThenBy(c => c.x).Should().BeEquivalentTo(new[] { (1, 1), (1, 2), (2, 1), (2, 2) });
        }
    }
}
