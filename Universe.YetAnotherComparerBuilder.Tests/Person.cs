using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Universe
{
    public class PersonClass
        {
            public string Title;
            public string Name;
            public int? Age;

            private sealed class PersonClassRelationalComparer : IComparer<PersonClass>
            {
                public int Compare(PersonClass x, PersonClass y)
                {
                    if (ReferenceEquals(x, y)) return 0;
                    if (ReferenceEquals(null, y)) return 1;
                    if (ReferenceEquals(null, x)) return -1;
                    
                    bool TitleProjection(PersonClass p) => p.Title != null && p.Title.StartsWith("Dr", StringComparison.InvariantCultureIgnoreCase);  
                    bool AgeProjection(PersonClass p) => p.Age != null && p.Age.Value == 42;  

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

            public static IComparer<PersonClass> PersonClassComparer { get; } = new PersonClassRelationalComparer();


            public PersonClass(string title, string name, int? age = null)
            {
                Title = title;
                Name = name;
                Age = age;
            }

            public override string ToString()
            {
                return $"{nameof(Title)}: {Title ?? "<null>"}, {nameof(Name)}: {Name}, {nameof(Age)}: {Age}";
            }

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

    }
}
