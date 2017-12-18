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
            StringComparer ignoreCase = StringComparer.InvariantCultureIgnoreCase;
            string highlightTitle = "Dr.";
            int highlightAge = 42;

            IComparer<Person> comparer = new ComparerBuilder<Person>()
                .Compare(x => ignoreCase.Equals(highlightTitle, x.Title))
                .Compare(x => x.Age.HasValue && highlightAge.Equals(x.Age.Value))
                .CompareString(x => x.Name, ignoreCase)
                .CompareString(x => x.Title, ignoreCase)
                .Compare(x => x.Age, OrderFlavour.Backward)
                .GetComparer();

            Enumerable.Empty<Person>().ToList().Sort(comparer);
            Enumerable.Empty<Person>().OrderBy(x => x, comparer);
        }
    }
}
