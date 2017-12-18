# Universe.YetAnotherComparerBuilder
Emit-free, fast, strongly typed, highly-customizable yet another ComparerBuilder

## The icing on
Extended order direction can be applyed to both an element comparer and a field comparer:
```csharp
[Flags]
public enum FieldOrder : byte
{
    Forward = 0,
    Backward = 1,
    NullGoesFinally = 0,
    NullGoesEarly = 2,
    Default = Forward | NullGoesFinally,
}
```

# Sample
This sample place all Doctors aged 42 on the early. after that all another `Person` follows:
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
```
