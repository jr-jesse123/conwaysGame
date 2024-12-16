//global using GameBoard = bool[,];
//global using LivePopulation = (int x, int y)[]; //TODO: duplicate global using
using ConwaysGame.Core;
using FluentAssertions;

namespace ConwaysGame.Tests
{
    public class GameTests
    {

        [Fact]
        public void SingleCellDies()
        {
            var inputs = from x in Enumerable.Range(0, 3)
                         from y in Enumerable.Range(0, 3)
                         select (x, y);

            foreach (var input in inputs)
            {
                var game = new Core.Game([input], 9);
                game.AdvanceGenerations(1);
                game.LiveCeels.Count.Should().Be(0);
            }
        }

        [Fact]
        public void LiveCellsWithTwoLivingNeighborsKeepAlive()
        {
            List<(int, int)[]> inputs =
                [
                    [(1, 0), (1, 1), (1, 2)], // Horizontal
                    [(0, 1), (1, 1), (2, 1)], // Vertical
                    [(0, 0), (1, 1), (2, 2)], // Diagonal
                    [(0, 2), (1, 1), (2, 0)], // Diagonal
                    [(0, 0), (0, 1), (1, 0)], // Corner
                    [(0, 2), (0, 1), (1, 2)], // Corner
                    [(2, 0), (2, 1), (1, 0)], // Corner
                    [(2, 2), (2, 1), (1, 2)], // Corner
                ];

            foreach (var input in inputs)
            {
                var game = new Core.Game(input, 9);
                game.AdvanceGenerations(1);
                game.LiveCeels.Single(c => c == input[1]);
            }
        }


        [Fact]
        public void DeadCellsWithThreeLivingNeighborsAreBorn()
        {
            List<((int, int)[] input, (int, int)[] expected)> inputs =
            [

                ([(1,0), (1, 1), (1, 2)], [(0,1), (2, 1)]), // Horizontal
                ([(0, 1), (1, 1), (2, 1)], [(1, 0), (1, 2)]), // Vertical
                ([(0, 0), (0, 1), (1, 0)], [(1, 1)]), // Corner
                ([(0, 2), (0, 1), (1, 2)], [(1, 1)]), // Corner
                ([(2, 0), (2, 1), (1, 0)], [(1, 1)]), // Corner
                ([(2, 2), (2, 1), (1, 2)], [(1, 1)]), // Corner
            ];


            foreach (var input in inputs)
            {
                var game = new Core.Game(input.input, 9);
                game.AdvanceGenerations(1);

                input.expected.Should().BeSubsetOf(game.LiveCeels);

            }
        }


        [Fact]
        public void LiveCellsWithMoreThanThreeLivingNeighborsDie()
        {
            var inputs = from x in Enumerable.Range(0, 3)
                         from y in Enumerable.Range(0, 3)
                         select (x, y);

            var game = new Core.Game(inputs, 9);

            game.AdvanceGenerations(1);

            game.LiveCeels.Count.Should().Be(4);
        }


        [Fact]
        public void LiveCellsWithLessThanTwoLiveNeighborsDie()
        {
            List<(int, int)[]> inputs =
               [
                   [(1, 0), (1, 2)], // Horizontal
                    [(0, 1), (2, 1)], // Vertical
                    [(0, 0), (2, 2)], // Diagonal
                    [(0, 2), (2, 0)], // Diagonal
                    [(0, 0), (1, 0)], // Corner
                    [(0, 2), (1, 2)], // Corner
                    [(2, 0), (1, 0)], // Corner
                    [(2, 2), (1, 2)], // Corner
                ];

            foreach (var input in inputs)
            {
                var game = new Core.Game(input, 9);
                game.AdvanceGenerations(1);
                game.LiveCeels.Any().Should().BeFalse();
            }

        }

        [Fact]
        public void ThorwsOnExcededGenerations()
        {
            var game = new Core.Game([], 9, 0);
            Action act = () => game.AdvanceGenerations(10);
            act.Should().Throw<MaxGenerationsReachedException>();
        }

        [Fact]
        public void CanRecognizeStableGeneration()
        {
            (int, int)[] input = [(0,0), (0, 1), (1, 0)];
            var game = new Core.Game(input, 9);

            while (!game.HasStabilized)
            {
                game.AdvanceGenerations(1);
            }

            game.LiveCeels.Count.Should().Be(4);
        }
    }
}