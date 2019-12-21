using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Universe.YetAnotherComparerBuilder.Tests;

namespace Universe.YetAnotherComparerBuilder.Benchmark
{
    [MemoryDiagnoser]
    public class SortingBenchmark
    {
        private PersonClass[] Persons;
        private static readonly IComparer<PersonClass> YAC_Comparer = PersonComparers.GetDemoComparer("Dr", 42);
        private static readonly IComparer<PersonClass> Manual_Comparer = PersonComparers.ManualPersonClassComparer;

        [GlobalSetup]
        public void Setup()
        {
            Persons = PersonsGenerator.Generate(666).ToArray();
        }

        [Benchmark]
        public void YAC_Sort()
        {
            var length = Persons.Length;
            var copy = new PersonClass[length];
            for (int i = 0; i < length; i++) copy[i] = Persons[i];
            Array.Sort(copy, YAC_Comparer);
        }

        [Benchmark]
        public void Manual_Sort()
        {
            var length = Persons.Length;
            var copy = new PersonClass[length];
            for (int i = 0; i < length; i++) copy[i] = Persons[i];
            Array.Sort(copy, Manual_Comparer);
        }

        [Benchmark]
        public int Linq_OrderBy()
        {
            var copy = Persons.Order().ToArray();
            return copy.Length;
        }
    }
}