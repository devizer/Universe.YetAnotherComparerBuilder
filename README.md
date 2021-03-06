# Universe.YetAnotherComparerBuilder
Emit-free, fast, strongly typed, highly-customizable yet another ComparerBuilder


<img src='images/yet-another-standard.png' width='500px' height='283px'></img>

## The icing on the
Extended order direction can be applyed to both an element comparer and a field comparer:
```csharp
[Flags]
public enum OrderFlavour
{
    Forward = 0,
    Backward = 1,
    NullGoesFinally = 0,
    NullGoesEarly = 2,
    Default = Forward | NullGoesFinally,
}
```

# Sample
In this sample all Doctors aged 42 will ahead of another persons.
```csharp
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
            .Compare(x => x.Age.HasValue && highlightAge == x.Age.Value)
            .CompareString(x => x.Name, ignoreCase)
            .CompareString(x => x.Title, ignoreCase)
            .Compare(x => x.Age, OrderFlavour.Backward | OrderFlavour.NullGoesFinally)
            .GetComparer();

        Enumerable.Empty<Person>().ToList().Sort(comparer);
        Enumerable.Empty<Person>().OrderBy(x => x, comparer).ToList();
    }
}
```
