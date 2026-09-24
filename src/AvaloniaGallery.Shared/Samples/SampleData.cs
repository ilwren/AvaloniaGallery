using System.Collections.ObjectModel;

namespace AvaloniaGallery.Samples;

/// <summary>Row type used by the TableView / DataGrid / TreeView samples.</summary>
public sealed class Country
{
    public required string Name { get; init; }
    public required string Region { get; init; }
    public long Population { get; init; }
    public long Area { get; init; }
    public double Gdp { get; init; }

    public override string ToString() => Name;
}

public sealed class Person
{
    public required string First { get; init; }
    public required string Last { get; init; }
    public int Age { get; init; }
    public string Full => $"{First} {Last}";
    public override string ToString() => Full;
}

/// <summary>Hierarchical node for TreeView samples.</summary>
public sealed class Node
{
    public required string Title { get; init; }
    public ObservableCollection<Node> Children { get; } = new();
    public override string ToString() => Title;
}

/// <summary>
/// Shared, read-only sample data. Every collection is materialised once and handed to the
/// samples that need it, so switching pages never pays for rebuilding the same lists.
/// </summary>
public static class SampleData
{
    public static ObservableCollection<Country> Countries { get; } = new(new[]
    {
        new Country { Name = "China",         Region = "Asia",          Population = 1_425_671_352, Area = 9_596_961, Gdp = 17_963_170 },
        new Country { Name = "India",         Region = "Asia",          Population = 1_428_627_663, Area = 3_287_263, Gdp =  3_416_646 },
        new Country { Name = "United States", Region = "Americas",      Population =   339_996_563, Area = 9_833_517, Gdp = 25_462_700 },
        new Country { Name = "Indonesia",     Region = "Asia",          Population =   277_534_122, Area = 1_904_569, Gdp =  1_319_100 },
        new Country { Name = "Pakistan",      Region = "Asia",          Population =   240_485_658, Area =   881_913, Gdp =    376_493 },
        new Country { Name = "Nigeria",       Region = "Africa",        Population =   223_804_632, Area =   923_768, Gdp =    477_386 },
        new Country { Name = "Brazil",        Region = "Americas",      Population =   216_422_446, Area = 8_515_767, Gdp =  1_920_096 },
        new Country { Name = "Bangladesh",    Region = "Asia",          Population =   172_954_319, Area =   147_570, Gdp =    460_201 },
        new Country { Name = "Russia",        Region = "Europe",        Population =   144_444_359, Area = 17_098_246, Gdp = 2_240_422 },
        new Country { Name = "Mexico",        Region = "Americas",      Population =   128_455_567, Area = 1_964_375, Gdp =  1_414_187 },
        new Country { Name = "Japan",         Region = "Asia",          Population =   123_294_513, Area =   377_930, Gdp =  4_231_141 },
        new Country { Name = "Germany",       Region = "Europe",        Population =    83_294_633, Area =   357_114, Gdp =  4_072_192 },
        new Country { Name = "France",        Region = "Europe",        Population =    64_756_584, Area =   551_695, Gdp =  2_782_905 },
        new Country { Name = "United Kingdom",Region = "Europe",        Population =    67_736_802, Area =   242_495, Gdp =  3_070_668 },
        new Country { Name = "Australia",     Region = "Oceania",       Population =    26_439_111, Area = 7_692_024, Gdp =  1_675_419 },
    });

    public static ObservableCollection<Person> People { get; } = new(new[]
    {
        new Person { First = "Ada",     Last = "Lovelace",  Age = 36 },
        new Person { First = "Grace",   Last = "Hopper",    Age = 85 },
        new Person { First = "Alan",    Last = "Turing",    Age = 41 },
        new Person { First = "Katherine", Last = "Johnson", Age = 101 },
        new Person { First = "Linus",   Last = "Torvalds",  Age = 55 },
        new Person { First = "Barbara", Last = "Liskov",    Age = 85 },
        new Person { First = "Donald",  Last = "Knuth",     Age = 87 },
    });

    public static string[] Fruits { get; } =
    {
        "Apple", "Banana", "Blueberry", "Cherry", "Coconut", "Dragonfruit", "Fig",
        "Grape", "Kiwi", "Lemon", "Lime", "Mango", "Orange", "Papaya", "Peach",
        "Pear", "Pineapple", "Plum", "Raspberry", "Strawberry", "Watermelon",
    };

    public static string[] Cities { get; } =
    {
        "Amsterdam", "Berlin", "Chicago", "Dubai", "Edinburgh", "Florence", "Geneva",
        "Helsinki", "Istanbul", "Jakarta", "Kyoto", "Lisbon", "Madrid", "Nairobi",
        "Oslo", "Prague", "Quebec", "Rome", "Seoul", "Tokyo", "Vienna", "Zurich",
    };

    /// <summary>Numbers used by list virtualisation demos.</summary>
    public static ObservableCollection<string> ManyItems { get; } =
        new(Enumerable.Range(1, 10_000).Select(i => $"Item {i:N0}"));

    public static ObservableCollection<Node> Tree { get; } = BuildTree();

    private static ObservableCollection<Node> BuildTree()
    {
        Node N(string title, params Node[] children)
        {
            var n = new Node { Title = title };
            foreach (var c in children)
                n.Children.Add(c);
            return n;
        }

        return new ObservableCollection<Node>
        {
            N("Documents",
                N("Reports",
                    N("2024 Q3.pdf"),
                    N("2024 Q4.pdf")),
                N("Invoices",
                    N("March.xlsx"),
                    N("April.xlsx"))),
            N("Pictures",
                N("Holiday",
                    N("beach.jpg"),
                    N("sunset.jpg")),
                N("Screenshots")),
            N("Source",
                N("Avalonia",
                    N("src"),
                    N("tests"))),
        };
    }
}
