using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaGallery.Services;

/// <summary>
/// Projects a sample's XAML into the equivalent C#.
/// <para>
/// Hand writing a second copy of all 109 samples would guarantee the two drift apart the
/// first time one is edited. Generating the C# from the same string that is parsed and
/// displayed keeps the gallery's core promise intact: there is exactly one source of truth
/// per sample, and every pane is a view onto it.
/// </para>
/// <para>
/// Property values are rendered by reflecting over the real Avalonia types, so an enum comes
/// out as <c>Orientation.Horizontal</c> and a char as <c>'•'</c> rather than as a string that
/// would not compile. Constructs with no object-initialiser form — templates, style
/// selectors, event handlers — are emitted as a marked comment instead of plausible-looking
/// code that would mislead.
/// </para>
/// </summary>
public static class CSharpProjector
{
    private static readonly XNamespace Xaml = "http://schemas.microsoft.com/winfx/2006/xaml";

    /// <summary>
    /// Assemblies searched when resolving an element name to a real type. ColorPicker and
    /// DataGrid ship as separate packages, so they have to be listed explicitly or their
    /// controls resolve to null and every value degrades to a string.
    /// </summary>
    private static readonly Assembly[] ControlAssemblies =
    {
        typeof(Button).Assembly,               // Avalonia.Controls
        typeof(AvaloniaObject).Assembly,       // Avalonia.Base
        typeof(SolidColorBrush).Assembly,      // Avalonia.Base (media)
        typeof(ColorPicker).Assembly,          // Avalonia.Controls.ColorPicker
        typeof(DataGrid).Assembly,             // Avalonia.Controls.DataGrid
        typeof(Avalonia.Controls.Shapes.Path).Assembly,
    };

    private static readonly Dictionary<string, Type?> TypeCache = new(StringComparer.Ordinal);
    private static readonly object Gate = new();

    /// <summary>
    /// Where an element's children go, by base type. Checked most-derived first, so
    /// SelectingItemsControl finds "Items" before ContentControl could claim "Content".
    /// </summary>
    private static readonly (string BaseType, string Property)[] CollectionProperties =
    {
        ("Panel", "Children"),
        ("ItemsControl", "Items"),
        ("MenuFlyout", "Items"),
        ("MultiPage", "Pages"),
        ("Decorator", "Child"),      // Border, Viewbox, LayoutTransformControl…
        ("Popup", "Child"),
        ("TextBlock", "Inlines"),
        ("Span", "Inlines"),
        ("GradientBrush", "GradientStops"),
        ("DrawingGroup", "Children"),
    };

    /// <summary>
    /// Returns the C# equivalent of <paramref name="xaml"/>, or null when it could not be
    /// projected (in which case the sample simply keeps its hand written C#, or none).
    /// </summary>
    public static string? Project(string xaml)
    {
        XDocument doc;
        try
        {
            doc = XDocument.Parse(xaml);
        }
        catch (System.Xml.XmlException)
        {
            return null;
        }

        if (doc.Root is null)
            return null;

        // Namespaces are collected while emitting, so the snippet carries exactly the
        // usings it needs and can be pasted into a file as-is.
        var namespaces = new SortedSet<string>(StringComparer.Ordinal);

        string? body;
        lock (Gate)
        {
            _namespaces = namespaces;
            try
            {
                body = Emit(doc.Root, 0);
            }
            finally
            {
                _namespaces = null;
            }
        }

        if (body is null)
            return null;

        var builder = new StringBuilder();
        builder.AppendLine("// C# equivalent of the XAML tab, generated from the same markup.");

        foreach (var ns in namespaces)
            builder.Append("using ").Append(ns).AppendLine(";");

        if (namespaces.Count > 0)
            builder.AppendLine();

        builder.Append("var root = ").Append(body).Append(';');
        return builder.ToString();
    }

    /// <summary>Namespaces used by the projection currently being emitted.</summary>
    [ThreadStatic]
    private static SortedSet<string>? _namespaces;

    private static void Use(Type? type)
    {
        if (type is null || _namespaces is null)
            return;

        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        if (!string.IsNullOrEmpty(underlying.Namespace) &&
            underlying.Namespace!.StartsWith("Avalonia", StringComparison.Ordinal))
        {
            _namespaces.Add(underlying.Namespace);
        }

        // A generic such as AvaloniaList<Point> needs both namespaces.
        if (underlying.IsGenericType)
        {
            foreach (var arg in underlying.GetGenericArguments())
                Use(arg);
        }

        if (underlying.IsNested && underlying.DeclaringType is not null)
            Use(underlying.DeclaringType);
    }

    /// <summary>Emits one element as a C# object creation expression.</summary>
    private static string? Emit(XElement element, int depth)
    {
        // Runaway nesting would produce something unreadable; samples never go this deep.
        if (depth > 12)
            return null;

        var typeName = element.Name.LocalName;

        // A property-element such as <Grid.ColumnDefinitions> is handled by its parent.
        if (typeName.Contains('.'))
            return null;

        // Templates are declarative by nature: the object-initialiser form would be a
        // FuncDataTemplate with a builder lambda, which is a different shape from the markup
        // and cannot be derived from it mechanically. Say so instead of emitting a type name
        // that does not exist as a constructible class.
        if (IsTemplate(typeName))
            return $"/* <{typeName}> — build this with a FuncDataTemplate in code */ null!";

        var type = ResolveType(typeName);

        // An unresolvable element means the projection would be guesswork from here down.
        if (type is null)
            return null;

        Use(type);
        var indent = new string(' ', (depth + 1) * 4);
        var closeIndent = new string(' ', depth * 4);

        var setters = new List<string>();
        var classes = new List<string>();

        // --- attributes ---------------------------------------------------------------
        foreach (var attribute in element.Attributes())
        {
            if (attribute.IsNamespaceDeclaration)
                continue;

            if (attribute.Name.LocalName == "Classes" && attribute.Name.Namespace != Xaml)
            {
                classes.AddRange(attribute.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                continue;
            }

            var setter = EmitAttribute(attribute, type, typeName);
            if (setter is not null)
                setters.Add(setter);
        }

        // Classes is a read-only collection, so it uses initialiser syntax.
        if (classes.Count > 0)
            setters.Add($"Classes = {{ {string.Join(", ", classes.Select(Quote))} }}");

        // --- property elements (<Button.Flyout>…) --------------------------------------
        var childElements = element.Elements().ToList();

        foreach (var child in childElements.Where(c => c.Name.LocalName.Contains('.')))
        {
            var local = child.Name.LocalName;
            var declaring = local[..local.IndexOf('.')];
            var propertyName = local[(local.IndexOf('.') + 1)..];
            var inner = child.Elements().ToList();

            // <ToolTip.Tip> on a Button is an attached property, not Button.Tip. It is
            // attached when the declaring type is not this element's type or a base of it.
            var declaringType = ResolveType(declaring);
            var isAttached = declaringType is not null && type is not null &&
                             !declaringType.IsAssignableFrom(type) &&
                             declaringType.GetField(propertyName + "Property",
                                 BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) is not null;

            if (isAttached && inner.Count == 1)
            {
                Use(declaringType);
                var attachedValue = Emit(inner[0], depth + 1);
                if (attachedValue is not null)
                    setters.Add($"[{declaring}.{propertyName}Property] = {attachedValue}");
                continue;
            }

            // A resource dictionary is keyed, so it needs a dictionary initialiser.
            var slotType = FindPropertyType(type, propertyName, attached: false);
            if (slotType is not null && typeof(IResourceDictionary).IsAssignableFrom(slotType))
            {
                var entries = new List<string>();
                foreach (var resource in inner)
                {
                    var key = resource.Attribute(Xaml + "Key")?.Value;
                    var built = Emit(resource, depth + 2);
                    if (key is not null && built is not null)
                        entries.Add($"[{Quote(key)}] = {built}");
                }

                if (entries.Count > 0)
                    setters.Add(Collection(propertyName, entries, depth));

                continue;
            }

            if (inner.Count == 1)
            {
                var value = Emit(inner[0], depth + 1);
                if (value is not null)
                    setters.Add($"{propertyName} = {value}");
            }
            else if (inner.Count > 1)
            {
                var items = inner.Select(i => Emit(i, depth + 2)).OfType<string>().ToList();
                if (items.Count > 0)
                    setters.Add(Collection(propertyName, items, depth));
            }
        }

        // --- child content -------------------------------------------------------------
        var contentChildren = childElements.Where(c => !c.Name.LocalName.Contains('.')).ToList();

        if (contentChildren.Count > 0)
        {
            var property = ContentPropertyFor(type);
            var propertyType = FindPropertyType(type, property, attached: false);

            // Whether the slot takes one value or many is a fact about the property type,
            // not about its name: Viewbox.Child and DrawingImage.Drawing are both single,
            // Panel.Children and TextBlock.Inlines are both collections.
            var isCollection = propertyType is not null &&
                               propertyType != typeof(string) &&
                               typeof(IEnumerable).IsAssignableFrom(propertyType);

            if (!isCollection && contentChildren.Count == 1)
            {
                var value = Emit(contentChildren[0], depth + 1);
                if (value is not null)
                    setters.Add($"{property} = {value}");
            }
            else
            {
                var items = contentChildren.Select(c => Emit(c, depth + 2)).OfType<string>().ToList();
                if (items.Count > 0)
                {
                    // A single-value slot given several children is really a panel.
                    var name = isCollection ? property : "Children";

                    // A collection typed as an interface cannot be collection-initialised
                    // in place, so construct the concrete list Avalonia would have made.
                    if (isCollection && propertyType is not null &&
                        (propertyType.IsInterface || propertyType.IsAbstract))
                    {
                        var itemType = propertyType.IsGenericType
                            ? FriendlyName(propertyType.GetGenericArguments()[0])
                            : "Control";

                        _namespaces?.Add("Avalonia.Collections");
                        setters.Add(Collection($"{name} = new AvaloniaList<{itemType}>", items, depth,
                            assigned: true));
                    }
                    else
                    {
                        setters.Add(Collection(name, items, depth));
                    }
                }
            }
        }
        else if (!element.HasElements && !string.IsNullOrWhiteSpace(element.Value))
        {
            var text = element.Value.Trim();
            if (text.Length > 0)
                setters.Add($"Content = {Quote(text)}");
        }

        if (setters.Count == 0)
            return $"new {typeName}()";

        var joined = string.Join(",\n" + indent, setters);
        return $"new {typeName}\n{closeIndent}{{\n{indent}{joined},\n{closeIndent}}}";
    }

    private static string Collection(
        string name,
        IReadOnlyList<string> items,
        int depth,
        bool assigned = false)
    {
        var indent = new string(' ', (depth + 1) * 4);
        var inner = new string(' ', (depth + 2) * 4);
        var joined = string.Join(",\n" + inner, items);
        var head = assigned ? name : $"{name} =";
        return $"{head}\n{indent}{{\n{inner}{joined},\n{indent}}}";
    }

    /// <summary>Works out where an element's children belong for the resolved type.</summary>
    private static string ContentPropertyFor(Type? type)
    {
        if (type is null)
            return "Content";

        // Walk up from the type itself so the nearest base wins: a ListBox is both an
        // ItemsControl and (further up) a TemplatedControl, and Items is the right answer.
        for (var t = type; t is not null; t = t.BaseType)
        {
            foreach (var (baseName, property) in CollectionProperties)
            {
                if (t.Name == baseName)
                    return property;
            }
        }

        // Some hosts name their single child something else entirely.
        if (type.GetProperty("Child", BindingFlags.Public | BindingFlags.Instance) is not null)
            return "Child";

        if (type.GetProperty("Content", BindingFlags.Public | BindingFlags.Instance) is null &&
            type.GetProperty("Drawing", BindingFlags.Public | BindingFlags.Instance) is not null)
        {
            return "Drawing";
        }

        return "Content";
    }

    /// <summary>Turns one XAML attribute into a C# initialiser entry.</summary>
    private static string? EmitAttribute(XAttribute attribute, Type? ownerType, string ownerName)
    {
        var name = attribute.Name.LocalName;
        var value = attribute.Value;

        // x:Name is the control's Name property; other x: directives have no runtime form.
        if (attribute.Name.Namespace == Xaml)
            return name == "Name" ? $"Name = {Quote(value)}" : null;

        if (value.StartsWith("{", StringComparison.Ordinal))
        {
            // Columns expose Binding as a plain CLR property: there is no BindingProperty
            // field to target with the [!Owner.XProperty] indexer form.
            if (!name.Contains('.') &&
                ownerType?.GetField(name + "Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) is null &&
                ownerType?.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) is { } clrProperty &&
                clrProperty.CanWrite)
            {
                var trimmed = value.Trim('{', '}').Trim();
                if (trimmed.StartsWith("Binding", StringComparison.Ordinal))
                {
                    _namespaces?.Add("Avalonia.Data");
                    var path = trimmed["Binding".Length..].Split(',')[0].Trim();
                    return $"{name} = new Binding(\"{path}\")";
                }
            }

            return EmitMarkupExtension(name, value, ownerName);
        }

        // Attached properties: Grid.Row="1" → [Grid.RowProperty] = 1
        if (name.Contains('.'))
        {
            var parts = name.Split('.');
            var attachedOwner = ResolveType(parts[0]);
            Use(attachedOwner);
            var attachedType = FindPropertyType(attachedOwner, parts[1], attached: true);

            // Only real attached properties have an XxxProperty field to index.
            if (attachedOwner?.GetField(parts[1] + "Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) is null)
                return $"// {name}=\"{value}\" has no object-initialiser equivalent";

            return $"[{parts[0]}.{parts[1]}Property] = {LiteralFor(attachedType, value)}";
        }

        // Event handlers cannot appear in an object initialiser.
        if (IsEvent(ownerType, name))
            return $"// {name}=\"{value}\" is an event; subscribe to it in code instead";

        return $"{name} = {LiteralFor(FindPropertyType(ownerType, name, attached: false), value)}";
    }

    /// <summary>
    /// Projects the markup extensions the samples actually use. Anything else becomes a
    /// comment so the reader is told rather than misled.
    /// </summary>
    private static string? EmitMarkupExtension(string name, string value, string ownerName)
    {
        var body = value.Trim('{', '}').Trim();

        if (body.StartsWith("Binding", StringComparison.Ordinal))
        {
            _namespaces?.Add("Avalonia.Data");
            var path = body["Binding".Length..].Trim();

            // {Binding #Element.Property}
            if (path.StartsWith("#", StringComparison.Ordinal))
            {
                var withoutHash = path[1..];
                var dot = withoutHash.IndexOf('.');

                var elementName = (dot < 0 ? withoutHash : withoutHash[..dot]).Trim();
                var elementPath = dot < 0 ? "." : withoutHash[(dot + 1)..];

                var comma = elementPath.IndexOf(',');
                if (comma >= 0)
                    elementPath = elementPath[..comma];

                return $"[!{ownerName}.{name}Property] = new Binding(\"{elementPath.Trim()}\") {{ ElementName = \"{elementName}\" }}";
            }

            var cleanPath = path.Split(',')[0].Trim();
            return cleanPath.Length == 0
                ? $"[!{ownerName}.{name}Property] = new Binding()"
                : $"[!{ownerName}.{name}Property] = new Binding(\"{cleanPath}\")";
        }

        if (body.StartsWith("DynamicResource", StringComparison.Ordinal))
        {
            _namespaces?.Add("Avalonia.Markup.Xaml.MarkupExtensions");
            return $"[!{ownerName}.{name}Property] = new DynamicResourceExtension(\"{body["DynamicResource".Length..].Trim()}\")";
        }

        if (body.StartsWith("StaticResource", StringComparison.Ordinal))
            return $"{name} = (dynamic)this.FindResource(\"{body["StaticResource".Length..].Trim()}\")!";

        return $"// {name}=\"{value}\" has no object-initialiser equivalent";
    }

    // ---------------------------------------------------------------------------------
    // Literals, resolved against the real property type wherever one can be found.
    // ---------------------------------------------------------------------------------

    private static string LiteralFor(Type? propertyType, string value)
    {
        if (propertyType is null)
            return Fallback(value);

        var underlying = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        Use(underlying);

        if (underlying.IsEnum)
        {
            // Nested enums such as PageSlide.SlideAxis need the declaring type to compile.
            var enumName = underlying.IsNested && underlying.DeclaringType is { } declaring
                ? $"{declaring.Name}.{underlying.Name}"
                : underlying.Name;

            // Flags come through as "Left, Right" in XAML.
            var parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => Enum.GetNames(underlying).Contains(p, StringComparer.Ordinal))
                .Select(p => $"{enumName}.{p}")
                .ToList();

            return parts.Count > 0 ? string.Join(" | ", parts) : Fallback(value);
        }

        // ThemeVariant exposes named statics rather than a Parse method.
        if (underlying.Name == "ThemeVariant")
            return $"ThemeVariant.{value}";

        if (underlying == typeof(bool))
            return bool.TryParse(value, out var b) ? (b ? "true" : "false") : Fallback(value);

        if (underlying == typeof(char))
            return "'" + value.Replace("\\", "\\\\").Replace("'", "\\'") + "'";

        if (underlying == typeof(string))
            return Quote(value);

        if (underlying == typeof(double) || underlying == typeof(float))
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _)
                ? value
                : Quote(value);
        }

        if (underlying == typeof(int) || underlying == typeof(long))
            return int.TryParse(value, out _) ? value : Quote(value);

        // NumericUpDown and friends are decimal, and a bare double literal will not compile.
        if (underlying == typeof(decimal))
            return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _)
                ? value + "m"
                : Quote(value);

        if (underlying == typeof(Thickness))
            return $"new Thickness({value.Replace(" ", string.Empty)})";

        if (underlying == typeof(CornerRadius))
            return $"new CornerRadius({value.Replace(" ", string.Empty)})";

        if (underlying == typeof(GridLength))
            return $"GridLength.Parse(\"{value}\")";

        if (underlying == typeof(Uri))
            return $"new Uri(\"{value}\")";

        // DataGridLength has no Parse overload taking only a string; it converts from
        // a double, or from the star/auto forms via its type converter.
        if (underlying.Name == "DataGridLength")
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var pixels))
                return $"new DataGridLength({pixels.ToString(CultureInfo.InvariantCulture)})";

            var text = value.Trim();
            if (string.Equals(text, "Auto", StringComparison.OrdinalIgnoreCase))
                return "DataGridLength.Auto";
            if (string.Equals(text, "SizeToCells", StringComparison.OrdinalIgnoreCase))
                return "DataGridLength.SizeToCells";
            if (string.Equals(text, "SizeToHeader", StringComparison.OrdinalIgnoreCase))
                return "DataGridLength.SizeToHeader";

            var starText = text.TrimEnd('*');
            var stars = starText.Length == 0
                ? 1d
                : double.TryParse(starText, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) ? parsed : 1d;

            return $"new DataGridLength({stars.ToString(CultureInfo.InvariantCulture)}, DataGridLengthUnitType.Star)";
        }

        if (typeof(IBrush).IsAssignableFrom(underlying))
            return $"Brush.Parse(\"{value}\")";

        if (underlying == typeof(Color))
            return $"Color.Parse(\"{value}\")";

        if (typeof(Geometry).IsAssignableFrom(underlying))
            return $"Geometry.Parse(\"{value}\")";

        if (underlying == typeof(TimeSpan))
            return $"TimeSpan.Parse(\"{value}\")";

        // Types that expose a static Parse(string) can be projected faithfully without the
        // projector needing to know anything about them — Point, RelativePoint, HsvColor,
        // BoxShadows, KeyGesture, ThemeVariant, ColumnDefinitions and more all qualify.
        if (HasStringParse(underlying))
            return $"{FriendlyName(underlying)}.Parse(\"{value}\")";

        // Numeric collections: StrokeDashArray is AvaloniaList<double>, Points is IList<Point>.
        // Both are declared through interfaces or generics that cannot be newed up directly,
        // so use the concrete type Avalonia itself instantiates.
        if (typeof(IEnumerable).IsAssignableFrom(underlying) && underlying != typeof(string))
        {
            var parts = value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var numbers = parts.All(p =>
                double.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture, out _));

            if (parts.Length > 0 && numbers)
            {
                var element = underlying.IsGenericType
                    ? underlying.GetGenericArguments()[0]
                    : typeof(double);

                // IList<Point> is populated two numbers at a time.
                if (element == typeof(Point))
                {
                    var points = new List<string>();
                    for (var i = 0; i + 1 < parts.Length; i += 2)
                        points.Add($"new Point({parts[i]}, {parts[i + 1]})");

                    _namespaces?.Add("Avalonia");
                    return $"new Points {{ {string.Join(", ", points)} }}";
                }

                string concrete;
                if (underlying.IsInterface || underlying.IsAbstract)
                {
                    concrete = $"AvaloniaList<{FriendlyName(element)}>";
                    _namespaces?.Add("Avalonia.Collections");
                }
                else
                {
                    concrete = FriendlyName(underlying);
                }

                return $"new {concrete} {{ {string.Join(", ", parts)} }}";
            }
        }

        // Something structured we do not special case: show the XAML text, which is at
        // least unambiguous about what was meant.
        return Fallback(value);
    }

    /// <summary>Template elements have no object-initialiser form.</summary>
    private static bool IsTemplate(string typeName) =>
        typeName.EndsWith("Template", StringComparison.Ordinal) ||
        typeName is "ControlTheme" or "Style" or "Styles" or "Setter";

    private static bool HasStringParse(Type type) =>
        type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null,
            new[] { typeof(string) }, null) is not null;

    /// <summary>Renders a type name usable in source, including generic arguments.</summary>
    private static string FriendlyName(Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        var name = type.Name[..type.Name.IndexOf('`')];
        var args = string.Join(", ", type.GetGenericArguments().Select(FriendlyName));
        return $"{name}<{args}>";
    }

    /// <summary>Best effort when the property type is unknown.</summary>
    private static string Fallback(string value)
    {
        if (bool.TryParse(value, out var b))
            return b ? "true" : "false";

        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            return value;

        return Quote(value);
    }

    private static Type? FindPropertyType(Type? owner, string name, bool attached)
    {
        if (owner is null)
            return null;

        if (!attached)
        {
            var clr = owner.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (clr is not null)
                return clr.PropertyType;
        }

        // Attached properties have no CLR property on the owner; read the registered
        // AvaloniaProperty's value type instead.
        var field = owner.GetField(name + "Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        if (field?.GetValue(null) is AvaloniaProperty avaloniaProperty)
            return avaloniaProperty.PropertyType;

        return null;
    }

    private static bool IsEvent(Type? owner, string name) =>
        owner?.GetEvent(name, BindingFlags.Public | BindingFlags.Instance) is not null;

    private static string Quote(string value) =>
        "\"" + value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", string.Empty)
            .Replace("\n", "\\n") + "\"";

    private static Type? ResolveType(string name)
    {
        lock (Gate)
        {
            if (TypeCache.TryGetValue(name, out var cached))
                return cached;

            Type? found = null;

            foreach (var assembly in ControlAssemblies)
            {
                found = assembly.GetTypes().FirstOrDefault(
                    t => t.IsPublic && t.Name == name && !t.IsGenericTypeDefinition);

                if (found is not null)
                    break;
            }

            TypeCache[name] = found;
            return found;
        }
    }
}
