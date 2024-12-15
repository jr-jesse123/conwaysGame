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

        //}

        [Fact]
        public void test()
        {
            
            var x = GenerateAllCombinations(8)
                .Select(k =>
                {
                    GameBoard board = new bool[3, 3];

                    for (int i = 0; i < k.Length; i++)
                    {
                        var _i = i;

                        if (_i == 4) _i++;

                        var line = _i % 3;
                        
                        var column = _i - line;

                        Console.WriteLine("line:" + line.ToString() + " column:" + column.ToString());

                        board[line, column] = k[i];
                    }


                    return board;
                })
                .ToArray();

            
            
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