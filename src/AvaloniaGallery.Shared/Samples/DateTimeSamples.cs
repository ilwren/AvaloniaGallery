using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

internal static class DateTimeSamples
{
    public static ControlCategory Build() => new ControlCategory
    {
        Name = "Date and time",
        IconData = Icons.DateTime,
        NameKey = "Cat.DateTime",
        SourceFile = "DateTimeSamples.cs",
    }
    .With(CalendarPage(), CalendarDatePickerPage(), DatePickerPage(), TimePickerPage());

    private static ControlPage CalendarPage() => new ControlPage
    {
        Name = "Calendar",
        Summary = "A full month view for picking one date, a range, or several dates.",
        Samples =
        {
            new ControlSample
            {
                Title = "Selection modes",
                Description = "SingleDate, SingleRange and MultipleRange cover the common cases.",
                PreviewHeight = 300,
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="20">
                  <StackPanel Spacing="6">
                    <TextBlock Text="Single date" FontWeight="SemiBold" FontSize="12" />
                    <Calendar SelectionMode="SingleDate" />
                  </StackPanel>
                  <StackPanel Spacing="6">
                    <TextBlock Text="Range" FontWeight="SemiBold" FontSize="12" />
                    <Calendar SelectionMode="SingleRange" />
                  </StackPanel>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage CalendarDatePickerPage() => new ControlPage
    {
        Name = "CalendarDatePicker",
        Summary = "A text field with a drop-down calendar attached.",
        Samples =
        {
            new ControlSample
            {
                Title = "Picking a date",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12" Width="260">
                  <CalendarDatePicker PlaceholderText="Select a date" />
                  <CalendarDatePicker SelectedDateFormat="Long" PlaceholderText="Long format" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage DatePickerPage() => new ControlPage
    {
        Name = "DatePicker",
        Summary = "Three spinners for day, month and year. Familiar from touch platforms.",
        Samples =
        {
            new ControlSample
            {
                Title = "Day, month, year",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12">
                  <DatePicker />
                  <DatePicker DayVisible="False" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage TimePickerPage() => new ControlPage
    {
        Name = "TimePicker",
        Summary = "Spinners for hour and minute, in 12 or 24 hour form.",
        Samples =
        {
            new ControlSample
            {
                Title = "12 and 24 hour clocks",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12">
                  <TimePicker ClockIdentifier="12HourClock" />
                  <TimePicker ClockIdentifier="24HourClock" MinuteIncrement="15" />
                </StackPanel>
                """,
            },
        },
    };
}
