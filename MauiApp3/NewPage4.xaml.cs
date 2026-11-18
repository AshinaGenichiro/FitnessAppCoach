using System.Collections.ObjectModel;
using System.Text.Json;

namespace MauiApp3;

public partial class NewPage4 : ContentPage
{
    private int seconds = 0;
    private bool completed = false;

    private const string StorageKey = "WorkoutProgress";

    private bool isLoading = false;

    public ObservableCollection<string> ExerciseDetails { get; set; } =
        new ObservableCollection<string>
        {
            "Equipment: Leg press machine",
            "Muscle group: Legs",
            "Target: Quadriceps, glutes and hamstrings",
            "Recommended rest: 60-90 seconds",
            "Keep your back against the seat",
            "Avoid locking your knees"
        };

    public NewPage4()
    {
        InitializeComponent();

        BindingContext = this;

        LoadProgress();

        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            seconds++;

            TimerLabel.Text = TimeSpan
                .FromSeconds(seconds)
                .ToString(@"hh\:mm\:ss");

            SaveProgress();

            return true;
        });
    }
    private void HelpButton_Clicked(object sender, EventArgs e)
    {
        HelpDetails.IsVisible = !HelpDetails.IsVisible;

        HelpButton.Text = HelpDetails.IsVisible
            ? "ⓘ Exercise Help - Hide details ▲"
            : "ⓘ Exercise Help - Show details ▼";
    }
    private void AddSet_Clicked(object sender, EventArgs e)
    {
        int number = SetsLayout.Children.Count + 1;

        VerticalStackLayout setLayout = new VerticalStackLayout
        {
            Spacing = 5
        };

        Label setLabel = new Label
        {
            Text = $"Set {number}",
            FontAttributes = FontAttributes.Bold
        };

        Entry weight = new Entry
        {
            Placeholder = "Weight (kg)",
            Keyboard = Keyboard.Numeric
        };

        Entry reps = new Entry
        {
            Placeholder = "Reps",
            Keyboard = Keyboard.Numeric
        };

        Grid grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10
        };

        grid.Add(weight);
        grid.Add(reps, 1);

        Button removeButton = new Button
        {
            Text = "✕ Remove Set",
            BackgroundColor = Color.FromArgb("#FEE2E2"),
            TextColor = Color.FromArgb("#B91C1C"),
            FontSize = 12
        };

        removeButton.Clicked += (s, args) =>
        {
            SetsLayout.Children.Remove(setLayout);

            UpdateSetNumbers();
            SaveProgress();
        };

        setLayout.Children.Add(setLabel);
        setLayout.Children.Add(grid);
        setLayout.Children.Add(removeButton);

        SetsLayout.Children.Add(setLayout);

        SaveProgress();
    }
    private void UpdateSetNumbers()
    {
        int number = 1;

        foreach (var child in SetsLayout.Children)
        {
            if (child is VerticalStackLayout layout &&
                layout.Children.FirstOrDefault() is Label label)
            {
                label.Text = $"Set {number}";
                number++;
            }
        }
    }
    private void MarkComplete_Clicked(object sender, EventArgs e)
    {
        completed = !completed;

        if (completed)
        {
            CompleteButton.Text = "✓ Completed";

            CompleteButton.BackgroundColor =
                Color.FromArgb("#86EFAC");

            CompleteButton.TextColor =
                Color.FromArgb("#166534");
        }
        else
        {
            CompleteButton.Text = "Mark As Complete";

            CompleteButton.BackgroundColor =
                Color.FromArgb("#D1D5DB");

            CompleteButton.TextColor =
                Color.FromArgb("#333333");
        }

        SaveProgress();
    }
    private void SaveProgress()
    {
        if (isLoading)
            return;

        var data = new WorkoutData
        {
            Seconds = seconds,
            Completed = completed,
            Sets = new List<SetData>()
        };

        foreach (var child in SetsLayout.Children)
        {
            if (child is VerticalStackLayout layout)
            {
                if (layout.Children.Count < 2 ||
                    layout.Children[1] is not Grid grid)
                    continue;

                var entries = grid.Children
                    .OfType<Entry>()
                    .ToList();

                if (entries.Count >= 2)
                {
                    data.Sets.Add(new SetData
                    {
                        Weight = entries[0].Text ?? "",
                        Reps = entries[1].Text ?? ""
                    });
                }
            }
        }

        Preferences.Default.Set(
            StorageKey,
            JsonSerializer.Serialize(data));
    }
    private void LoadProgress()
    {
        try
        {
            isLoading = true;

            string json = Preferences.Default.Get(
                StorageKey, "");

            if (string.IsNullOrEmpty(json))
                return;

            var data = JsonSerializer.Deserialize<WorkoutData>(json);

            if (data == null)
                return;

            seconds = data.Seconds;
            completed = data.Completed;

            TimerLabel.Text = TimeSpan
                .FromSeconds(seconds)
                .ToString(@"hh\:mm\:ss");

            if (completed)
            {
                CompleteButton.Text = "✓ Completed";

                CompleteButton.BackgroundColor =
                    Color.FromArgb("#86EFAC");

                CompleteButton.TextColor =
                    Color.FromArgb("#166534");
            }

            for (int i = 0; i < data.Sets.Count; i++)
            {
                if (i < SetsLayout.Children.Count)
                {
                    var layout = SetsLayout.Children[i]
                        as VerticalStackLayout;

                    if (layout?.Children[1] is Grid grid)
                    {
                        var entries = grid.Children
                            .OfType<Entry>()
                            .ToList();

                        entries[0].Text = data.Sets[i].Weight;
                        entries[1].Text = data.Sets[i].Reps;
                    }
                }
                else
                {
                    AddSet_Clicked(this, EventArgs.Empty);

                    var layout = SetsLayout.Children.Last()
                        as VerticalStackLayout;

                    if (layout?.Children[1] is Grid grid)
                    {
                        var entries = grid.Children
                            .OfType<Entry>()
                            .ToList();

                        entries[0].Text = data.Sets[i].Weight;
                        entries[1].Text = data.Sets[i].Reps;
                    }
                }
            }
        }
        catch (Exception)
        {
            // Brak poprawnych zapisanych danych.
        }
        finally
        {
            isLoading = false;
        }
    }
    public class WorkoutData
    {
        public int Seconds { get; set; }

        public bool Completed { get; set; }

        public List<SetData> Sets { get; set; } = new();
    }

    public class SetData
    {
        public string Weight { get; set; } = "";

        public string Reps { get; set; } = "";
    }
}