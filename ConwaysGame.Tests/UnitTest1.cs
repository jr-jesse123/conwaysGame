//global using GameBoard = bool[,];
//global using LivePopulation = (int x, int y)[]; //TODO: duplicate global using
using ConwaysGame.Core;
using FluentAssertions;
using System.Security.Cryptography.X509Certificates;

namespace ConwaysGame.Tests
{
    public class UnitTest1
    {
     
        [Fact]
        public void SingleCellDies()
        {
            var inputs = from x in Enumerable.Range(0, 3)
                         from y in Enumerable.Range(0, 3)
                         select (x, y);

            foreach (var input in inputs)
            {
                var game = new Game([input], 9);
                game.AdvanceGeneration();
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
                var game = new Game(input, 9);
                game.AdvanceGeneration();
                game.LiveCeels.Single(c => c == input[1]);
            }
        }


        [Fact]
        public void DeadCellsWithThreeLivingNeighborsAreBorn()
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
                var game = new Game(input, 9);
                game.AdvanceGeneration();
                game.LiveCeels.Single(c => c == input[1]);
            }
        }




        //}

        [Fact]
        public void test()
        {

            var lenght = 8;
            var x = GenerateAllCombinations(lenght)
                .Select<bool[], (int, int)[]>(k =>
                {
                    var sideLenght = Math.Sqrt(lenght);

                    (int, int)[] input = k.Select<bool,(int,int)?>((b, i) =>
                    {
                        if (b)
                        {
                            return ((int)(i % sideLenght), (int)(i / sideLenght));
                        }
                        else
                        {
                            return new Nullable<(int,int)>();
                        }
                    })
                    .Where(x => x is not null)
                    .Cast<(int, int)>()
                    .ToArray();


                    return input;
                });

            //var boardsAndExpectedLiveness = x.Select(coords =>
            //{
            //    return new Game(coords);
            //});

            //boardsAndExpectedLiveness
            //var boardsAndExpectedLiveness = x.Select(combination =>
            //{
            //    var board = new bool[3, 3];
            //    int index = 0;

            //    for (int row = 0; row < 3; row++)
            //    {
            //        for (int col = 0; col < 3; col++)
            //        {
            //            if (row == 1 && col == 1) continue; // Skip center position
            //            board[row, col] = combination[index++];
            //        }
            //    }

            //    return (board, combination.Count(k => k));
            //}).ToArray();



            //boardsAndExpectedLiveness.Should().AllSatisfy(x =>
            //{

            //    //GameBoardModule.GetLiveNeighbors(x.board, 1, 1).Should().Be(x.Item2);
            //});




        }

        private static IEnumerable<bool[]> GenerateAllCombinations(int length)
        {
            int totalCombinations = (int)Math.Pow(2, length);
            
            for (int i = 0; i < totalCombinations; i++)
            {
                bool[] combination = new bool[length];
                for (int j = 0; j < length; j++)
                {
                    combination[j] = ((i >> j) & 1) == 1;
                }
                yield return combination;
            }
        }
        


        //public static IEnumerable<object[]> boards
        //{
        //    get
        //    {
        //        (bool[], int)[] CombinationsAndResults = 
        //            GenerateAllCombinations(8)

        //            .Select(x => (x, x.Count(cell => cell))).ToArray();


        //    }
        //}
    }
}