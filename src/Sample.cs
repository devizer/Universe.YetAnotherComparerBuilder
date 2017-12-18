using System;
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

            StringComparer c = StringComparer.InvariantCultureIgnoreCase;
            IComparer<Person> comparer = new ComparerBuilder<Person>()
                .Add(x => c.Equals(highlighTitle, x.Title) ? 0 : 1)
                .Add(x => x.Age.HasValue && highlightAge.Equals(x.Age.Value) ? 0 : 1)
                .AddString(x => x.Name, c)
                .AddString(x => x.Title, c)
                .Add(x => x.Age, FieldOrder.Backward)
                .GetComparer();

            Enumerable.Empty<Person>().ToList().Sort(comparer);
            Enumerable.Empty<Person>().OrderBy(x => x, comparer);
        }
    }
}
