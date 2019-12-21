using System;
using System.Collections.Generic;
using System.Linq;

namespace Universe.YetAnotherComparerBuilder.Tests
{
    public static class PersonComparers
    {
        
        // 1st: Implementation
        public static IComparer<PersonClass> GetDemoComparer(string highlightTitle = "Dr", int highlightAge = 42)
        {
            StringComparison ignoring = StringComparison.InvariantCultureIgnoreCase;
            StringComparer ignore = StringComparer.InvariantCultureIgnoreCase;
            return new ComparerBuilder<PersonClass>()
                .Match(x => x.Title?.StartsWith(highlightTitle, ignoring) ?? false)
                .Match(x => x.Age.HasValue && highlightAge == x.Age.Value)
                .CompareString(x => x.Name, ignore)
                .CompareString(x => x.Title, ignore)
                .Compare(x => x.Age, OrderFlavour.Backward | OrderFlavour.NullGoesFinally)
                .GetComparer();
        }


        static bool TitleProjection(PersonClass p) => p.Title != null && p.Title.StartsWith("Dr", StringComparison.InvariantCultureIgnoreCase);
        static bool AgeProjection(PersonClass p) => p.Age != null && p.Age.Value == 42;
        // 2nd implementation
        private sealed class PersonClassRelationalComparer : IComparer<PersonClass>
        {
            public int Compare(PersonClass x, PersonClass y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;
                    
                var isDrComparison = -TitleProjection(x).CompareTo(TitleProjection(y));
                if (isDrComparison != 0) return isDrComparison;

                var ageComparison = -AgeProjection(x).CompareTo(AgeProjection(y));
                if (ageComparison != 0) return ageComparison;

                var nameComparison = string.Compare(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase);
                if (nameComparison != 0) return nameComparison;
                    
                var titleComparison = string.Compare(x.Title, y.Title, StringComparison.InvariantCultureIgnoreCase);
                if (titleComparison != 0) return titleComparison;
                    
                // Null Age goes finally 
                if (x.Age == null && y.Age == null) return 0;
                else if (y.Age == null) return -1;
                else if (x.Age == null) return 1;

                return x.Age.Value.CompareTo(y.Age.Value);
            }
        }

        public static IComparer<PersonClass> ManualPersonClassComparer { get; } = new PersonClassRelationalComparer();
        
        // 3rd implementation (linq)
        public static IEnumerable<PersonClass> Order(this IEnumerable<PersonClass> persons)
        {
            return persons
                .OrderByDescending(p => p.Title != null && p.Title.Equals("Dr", StringComparison.InvariantCultureIgnoreCase))
                .ThenByDescending(p => p.Age != null && p.Age == 42)
                .ThenBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
                .ThenBy(x => x.Title, StringComparer.InvariantCultureIgnoreCase)
                .ThenBy(x => x.Age ?? int.MaxValue);
        }


    }
}