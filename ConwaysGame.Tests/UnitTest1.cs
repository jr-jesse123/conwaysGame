global using GameBoard = bool[,];
global using LivePopulation = (int x, int y)[]; //TODO: duplicate global using
using ConwaysGame.Core;
using FluentAssertions;
using System.Security.Cryptography.X509Certificates;

namespace ConwaysGame.Tests
{
    public class UnitTest1
    {
        //[Theory]
        //[MemberData(nameof(boards))]
        //public void CorrectNieghborNumber(GameBoard board, int expectedNeighbors)
        //{
        //    GameBoardModule.GetLiveNeighbors(board, 1, 1).Should().Be(expectedNeighbors);


        [Fact]
        public void test2()
        { 
            //GameBoard2 gameBoard2 = new GameBoard2();

            

            //Span<bool> bools = stackalloc bool[x];
        }


            //}

            [Fact]
        public void test()
        {
            
            var x = GenerateAllCombinations(8);
            var boardsAndExpectedLiveness = x.Select(combination =>
            {
                var board = new bool[3, 3];
                int index = 0;
                
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (row == 1 && col == 1) continue; // Skip center position
                        board[row, col] = combination[index++];
                    }
                }
                
                return (board, combination.Count(k => k));
            }).ToArray();



            boardsAndExpectedLiveness.Should().AllSatisfy(x =>
            {
                GameBoardModule.GetLiveNeighbors(x.board, 1, 1).Should().Be(x.Item2);
            });




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