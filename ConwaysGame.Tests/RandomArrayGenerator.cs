using System;
using System.Collections.Generic;
using System.Linq;

namespace ConwaysGame.Core
{
    public static class IEnumereableExtensions
    {
        public static T[] GetRandomItems<T>(this IEnumerable<T> input, int length)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

            var inputArray = input.ToArray();
            var random = new Random();
            var result = new T[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = inputArray[random.Next(inputArray.Length)];
            }

            return result;
        }
    }
}
