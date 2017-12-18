﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Universe
{

    public class Demo
    {
        class Person
        {
            public string Title;
            public string Name;
            public int? Age;
        }

        public static void Test()
        {
            string highlighTitle = "Dr.";
            int highlightAge = 42;

            StringComparer ignoreCase = StringComparer.InvariantCultureIgnoreCase;
            IComparer<Person> comparer = new ComparerBuilder<Person>()
                .Compare(x => ignoreCase.Equals(highlighTitle, x.Title))
                .Compare(x => x.Age.HasValue && highlightAge.Equals(x.Age.Value))
                .CompareString(x => x.Name, ignoreCase)
                .CompareString(x => x.Title, ignoreCase)
                .Compare(x => x.Age, FieldOrder.Backward)
                .GetComparer();

            Enumerable.Empty<Person>().ToList().Sort(comparer);
            Enumerable.Empty<Person>().OrderBy(x => x, comparer);
        }
    }
}