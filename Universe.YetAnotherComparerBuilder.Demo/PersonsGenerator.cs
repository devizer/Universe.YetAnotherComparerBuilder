using System;
using System.Collections.Generic;
using System.Linq;

namespace Universe.YetAnotherComparerBuilder.Benchmark
{
    public class PersonsGenerator
    {
        static readonly Random rand = new Random(42);

        static string GetRandomString(int minLength, int maxLength) =>
            new string(Enumerable.Range(minLength, maxLength).Select(x => (char) rand.Next(65, 90)).ToArray());
        
        public static IEnumerable<PersonClass> Generate(int count)
        {
            var titles = new[] {"Dr", "Mr", "Ms", null};
            for (int i = 0; i < count; i++)
            {
                yield return new PersonClass(
                    rand.Next(3) == 0 ? "Dr" : titles[rand.Next(titles.Length)],
                    // "Dr",
                    GetRandomString(20, 30),
                    rand.Next(3) == 0 ? 42 : rand.Next(3) == 0 ? (int?) null : rand.Next(20, 60)
                    // 42
                );
            }
        }
    }
}