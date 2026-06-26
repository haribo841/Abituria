using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Abituria;

public partial class MainWindow : Window
{
    private readonly ProfileStore _profileStore = new();
    private readonly List<FormulaItem> _formulas = BuildFormulas();
    private readonly List<TaskImage> _tasks = BuildTaskImages();
    private Border _shellHost = null!;
    private string? _activeProfile;

    public MainWindow()
    {
        InitializeComponent();
        _shellHost = this.FindControl<Border>("ShellHost")
            ?? throw new InvalidOperationException("ShellHost was not found.");
        ShowLogin();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void RenderShell(Control body, string selectedSection)
    {
        var root = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*"),
            Background = Brush("#F7F4ED")
        };

        root.Children.Add(BuildTopBar(selectedSection));
        Grid.SetRow(body, 1);
        root.Children.Add(body);

        _shellHost.Child = root;
    }

    private Control BuildTopBar(string selectedSection)
    {
        var header = new Border
        {
            Background = Brush("#FFFDF8"),
            BorderBrush = Brush("#E1DFD6"),
            BorderThickness = new Avalonia.Thickness(0, 0, 0, 1),
            Padding = new Avalonia.Thickness(22, 12),
        };

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
            ColumnSpacing = 20,
            VerticalAlignment = VerticalAlignment.Center
        };

        var brand = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            VerticalAlignment = VerticalAlignment.Center
        };
        brand.Children.Add(AssetImage("img/icon.png", 36));
        brand.Children.Add(new TextBlock
        {
            Text = "Abituria",
            FontSize = 26,
            FontWeight = FontWeight.Bold,
            Foreground = Brush("#17202A"),
            VerticalAlignment = VerticalAlignment.Center
        });
        grid.Children.Add(brand);

        var nav = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        AddNavButton(nav, "Start", "home", selectedSection, ShowHome);
        AddNavButton(nav, "Wzory", "formulas", selectedSection, ShowFormulas);
        AddNavButton(nav, "Zadania", "tasks", selectedSection, ShowTasks);
        AddNavButton(nav, "Kalkulator", "calculator", selectedSection, ShowCalculator);
        AddNavButton(nav, "Profil", "profile", selectedSection, ShowProfile);
        Grid.SetColumn(nav, 1);
        grid.Children.Add(nav);

        var profile = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10,
            VerticalAlignment = VerticalAlignment.Center
        };
        profile.Children.Add(new TextBlock
        {
            Text = _activeProfile is null ? "Nie zalogowano" : _activeProfile,
            Foreground = Brush("#637083"),
            VerticalAlignment = VerticalAlignment.Center
        });
        var logout = new Button { Content = "Wyloguj", Classes = { "ghost" } };
        logout.Click += (_, _) => ShowLogin();
        profile.Children.Add(logout);
        Grid.SetColumn(profile, 2);
        grid.Children.Add(profile);

        header.Child = grid;
        return header;
    }

    private static void AddNavButton(StackPanel nav, string label, string key, string selectedSection, Action action)
    {
        var button = new Button
        {
            Content = label,
            Classes = { key == selectedSection ? "primary" : "ghost" }
        };
        button.Click += (_, _) => action();
        nav.Children.Add(button);
    }

    private void ShowLogin()
    {
        _activeProfile = null;
        var profiles = _profileStore.LoadProfiles();

        var page = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("1.05*,0.95*"),
            Margin = new Avalonia.Thickness(34),
            ColumnSpacing = 28
        };

        var intro = Card(new Avalonia.Thickness(34), "#FFFFFF");
        var introStack = new StackPanel { Spacing = 18 };
        introStack.Children.Add(AssetImage("img/abituria.png", 210, 84));
        introStack.Children.Add(new TextBlock
        {
            Text = "Twój matematyczny korepetytor",
            Classes = { "h1" },
            TextWrapping = TextWrapping.Wrap
        });
        introStack.Children.Add(new TextBlock
        {
            Text = "Wybierz profil albo utwórz nowy. Aplikacja zapamięta profile lokalnie na tym urządzeniu i przeniesie Cię do menu nauki.",
            Classes = { "muted" }
        });
        introStack.Children.Add(BuildFeatureRow("img/wzory.png", "Wzory i działy", "Szybki przegląd najważniejszych tematów maturalnych."));
        introStack.Children.Add(BuildFeatureRow("img/kalkulator.png", "Kalkulator", "Rozwiązywanie funkcji kwadratowej z deltą i miejscami zerowymi."));
        introStack.Children.Add(BuildFeatureRow("img/matura.png", "Matura", "Podgląd przykładowych zadań z dołączonych materiałów."));
        intro.Child = introStack;
        page.Children.Add(intro);

        var panel = Card(new Avalonia.Thickness(28), "#FFFDF8");
        var stack = new StackPanel { Spacing = 14 };
        stack.Children.Add(new TextBlock { Text = "Logowanie", Classes = { "h2" } });
        stack.Children.Add(new TextBlock
        {
            Text = "Wybierz profil ucznia.",
            Classes = { "muted" }
        });

        var profileBox = new ComboBox
        {
            ItemsSource = profiles,
            SelectedIndex = profiles.Count > 0 ? 0 : -1,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        stack.Children.Add(profileBox);

        var status = new TextBlock
        {
            Foreground = Brush("#B7442E"),
            TextWrapping = TextWrapping.Wrap
        };

        var login = new Button
        {
            Content = "Wejdź do Abiturii",
            Classes = { "primary" },
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        login.Click += (_, _) =>
        {
            if (profileBox.SelectedItem is string profile && !string.IsNullOrWhiteSpace(profile))
            {
                _activeProfile = profile;
                ShowHome();
                return;
            }

            status.Text = "Najpierw wybierz profil.";
        };
        stack.Children.Add(login);

        stack.Children.Add(Separator());
        stack.Children.Add(new TextBlock { Text = "Nowy profil", Classes = { "h2" } });
        var nameInput = new TextBox
        {
            PlaceholderText = "Imię lub pseudonim",
            MaxLength = 15
        };
        stack.Children.Add(nameInput);

        var add = new Button
        {
            Content = "Dodaj profil",
            Classes = { "ghost" },
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        add.Click += (_, _) =>
        {
            var result = _profileStore.AddProfile(nameInput.Text ?? string.Empty);
            status.Text = result.Message;
            if (result.Success)
            {
                profiles = _profileStore.LoadProfiles();
                profileBox.ItemsSource = profiles;
                profileBox.SelectedItem = result.ProfileName;
                nameInput.Text = string.Empty;
            }
        };
        stack.Children.Add(add);
        stack.Children.Add(status);

        panel.Child = stack;
        Grid.SetColumn(panel, 1);
        page.Children.Add(panel);

        _shellHost.Child = page;
    }

    private static Control BuildFeatureRow(string asset, string title, string description)
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*"),
            ColumnSpacing = 14
        };
        grid.Children.Add(AssetImage(asset, 42));

        var text = new StackPanel { Spacing = 2 };
        text.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            Foreground = Brush("#17202A")
        });
        text.Children.Add(new TextBlock
        {
            Text = description,
            Classes = { "muted" }
        });
        Grid.SetColumn(text, 1);
        grid.Children.Add(text);
        return grid;
    }

    private void ShowHome()
    {
        var scroll = PageScroll();
        var stack = new StackPanel { Spacing = 22 };
        stack.Children.Add(PageTitle("Start", $"Cześć, {_activeProfile}. Wybierz, nad czym dziś pracujemy."));

        var cards = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto"),
            ColumnSpacing = 18,
            RowSpacing = 18
        };
        AddHomeCard(cards, 0, 0, "Wzory", "Przejrzyj najważniejsze wzory i pojęcia.", "img/wzory.png", ShowFormulas);
        AddHomeCard(cards, 1, 0, "Zadania", "Otwórz przykładowe arkusze i zadania maturalne.", "img/zadania.png", ShowTasks);
        AddHomeCard(cards, 0, 1, "Kalkulator", "Policz deltę, miejsca zerowe i wierzchołek paraboli.", "img/kalkulator.png", ShowCalculator);
        AddHomeCard(cards, 1, 1, "Działy", "Zobacz mapę tematów do powtórki.", "img/dzialy.png", ShowChapters);
        stack.Children.Add(cards);

        stack.Children.Add(InfoBand(
            "Dobra kolejność nauki",
            "Najpierw powtórz wzory, potem zrób kilka zadań z arkuszy, a kalkulator traktuj jako szybkie sprawdzenie rachunków."));

        scroll.Content = stack;
        RenderShell(scroll, "home");
    }

    private static void AddHomeCard(Grid grid, int column, int row, string title, string body, string asset, Action action)
    {
        var button = new Button
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Padding = new Avalonia.Thickness(0),
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent
        };
        button.Click += (_, _) => action();

        var card = Card(new Avalonia.Thickness(22), "#FFFFFF");
        var stack = new StackPanel { Spacing = 12 };
        stack.Children.Add(AssetImage(asset, 54));
        stack.Children.Add(new TextBlock { Text = title, Classes = { "h2" } });
        stack.Children.Add(new TextBlock { Text = body, Classes = { "muted" } });
        card.Child = stack;
        button.Content = card;

        Grid.SetColumn(button, column);
        Grid.SetRow(button, row);
        grid.Children.Add(button);
    }

    private void ShowFormulas()
    {
        var scroll = PageScroll();
        var stack = new StackPanel { Spacing = 18 };
        stack.Children.Add(PageTitle("Wzory", "Krótka karta powtórkowa z najważniejszymi tematami."));

        foreach (var formula in _formulas)
        {
            var card = Card(new Avalonia.Thickness(20), "#FFFFFF");
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("*,Auto"),
                ColumnSpacing = 18
            };
            var text = new StackPanel { Spacing = 8 };
            text.Children.Add(new TextBlock { Text = formula.Title, Classes = { "h2" } });
            text.Children.Add(new TextBlock
            {
                Text = formula.Description,
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brush("#333C47")
            });
            text.Children.Add(new TextBlock
            {
                Text = formula.Example,
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brush("#256D85"),
                FontWeight = FontWeight.SemiBold
            });
            grid.Children.Add(text);

            if (!string.IsNullOrWhiteSpace(formula.Asset))
            {
                var image = AssetImage(formula.Asset, 170, 110);
                Grid.SetColumn(image, 1);
                grid.Children.Add(image);
            }

            card.Child = grid;
            stack.Children.Add(card);
        }

        scroll.Content = stack;
        RenderShell(scroll, "formulas");
    }

    private void ShowTasks()
    {
        var scroll = PageScroll();
        var root = new StackPanel { Spacing = 18 };
        root.Children.Add(PageTitle("Zadania", "Podgląd zadań maturalnych dołączonych jako grafiki w projekcie."));

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("280,*"),
            ColumnSpacing = 18
        };

        var list = Card(new Avalonia.Thickness(14), "#FFFFFF");
        var listStack = new StackPanel { Spacing = 8 };
        listStack.Children.Add(new TextBlock
        {
            Text = "Arkusz 2021",
            FontWeight = FontWeight.Bold,
            FontSize = 18,
            Foreground = Brush("#17202A")
        });
        list.Child = new ScrollViewer { Content = listStack, MaxHeight = 470 };

        var previewCard = Card(new Avalonia.Thickness(18), "#FFFDF8");
        var preview = new StackPanel { Spacing = 12 };
        var previewTitle = new TextBlock { Text = _tasks[0].Title, Classes = { "h2" } };
        var previewImage = AssetImage(_tasks[0].Asset, 650, 360);
        var previewDescription = new TextBlock
        {
            Text = _tasks[0].Description,
            Classes = { "muted" }
        };
        preview.Children.Add(previewTitle);
        preview.Children.Add(previewImage);
        preview.Children.Add(previewDescription);
        previewCard.Child = preview;

        foreach (var task in _tasks)
        {
            var button = new Button
            {
                Content = task.Title,
                Classes = { "ghost" },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            button.Click += (_, _) =>
            {
                previewTitle.Text = task.Title;
                previewImage.Source = LoadBitmap(task.Asset);
                previewDescription.Text = task.Description;
            };
            listStack.Children.Add(button);
        }

        grid.Children.Add(list);
        Grid.SetColumn(previewCard, 1);
        grid.Children.Add(previewCard);
        root.Children.Add(grid);
        scroll.Content = root;
        RenderShell(scroll, "tasks");
    }

    private void ShowCalculator()
    {
        var scroll = PageScroll();
        var stack = new StackPanel { Spacing = 18 };
        stack.Children.Add(PageTitle("Kalkulator funkcji kwadratowej", "Podaj współczynniki równania ax² + bx + c = 0."));

        var card = Card(new Avalonia.Thickness(24), "#FFFFFF");
        var form = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto"),
            ColumnSpacing = 14,
            RowSpacing = 16
        };

        var aBox = NumberBox("a", "1");
        var bBox = NumberBox("b", "0");
        var cBox = NumberBox("c", "0");
        form.Children.Add(aBox);
        Grid.SetColumn(bBox, 1);
        form.Children.Add(bBox);
        Grid.SetColumn(cBox, 2);
        form.Children.Add(cBox);

        var calculate = new Button
        {
            Content = "Oblicz",
            Classes = { "primary" },
            HorizontalAlignment = HorizontalAlignment.Left,
            MinWidth = 160
        };
        Grid.SetRow(calculate, 1);
        form.Children.Add(calculate);

        var result = new TextBlock
        {
            Text = "Wynik pojawi się tutaj.",
            TextWrapping = TextWrapping.Wrap,
            FontSize = 18,
            Foreground = Brush("#17202A")
        };
        Grid.SetRow(result, 2);
        Grid.SetColumnSpan(result, 3);
        form.Children.Add(result);

        calculate.Click += (_, _) =>
        {
            result.Text = SolveQuadratic(aBox.Text, bBox.Text, cBox.Text);
        };

        card.Child = form;
        stack.Children.Add(card);
        stack.Children.Add(InfoBand("Wskazówka", "W polach możesz używać przecinka albo kropki jako separatora dziesiętnego."));

        scroll.Content = stack;
        RenderShell(scroll, "calculator");
    }

    private void ShowChapters()
    {
        var scroll = PageScroll();
        var stack = new StackPanel { Spacing = 18 };
        stack.Children.Add(PageTitle("Działy", "Mapa tematów do powtórki przed maturą."));

        var topics = new[]
        {
            ("Funkcje", "liniowa, kwadratowa, wykładnicza, logarytmiczna"),
            ("Równania i nierówności", "przekształcenia, układy, wartość bezwzględna"),
            ("Geometria", "planimetria, stereometria, trygonometria"),
            ("Ciągi", "arytmetyczne, geometryczne, suma wyrazów"),
            ("Prawdopodobieństwo", "kombinatoryka, zdarzenia, schemat klasyczny"),
            ("Analiza danych", "średnia, mediana, odchylenie i interpretacja wykresów")
        };

        foreach (var topic in topics)
        {
            stack.Children.Add(InfoBand(topic.Item1, topic.Item2));
        }

        scroll.Content = stack;
        RenderShell(scroll, "home");
    }

    private void ShowProfile()
    {
        var scroll = PageScroll();
        var stack = new StackPanel { Spacing = 18 };
        stack.Children.Add(PageTitle("Profil", "Informacje zapisane lokalnie w tej instalacji."));

        var profiles = _profileStore.LoadProfiles();
        stack.Children.Add(InfoBand("Aktywny profil", _activeProfile ?? "Brak aktywnego profilu."));
        stack.Children.Add(InfoBand("Liczba profili", profiles.Count.ToString(CultureInfo.InvariantCulture)));
        stack.Children.Add(InfoBand("Plik profili", _profileStore.FilePath));

        var logout = new Button
        {
            Content = "Wróć do logowania",
            Classes = { "primary" },
            HorizontalAlignment = HorizontalAlignment.Left
        };
        logout.Click += (_, _) => ShowLogin();
        stack.Children.Add(logout);

        scroll.Content = stack;
        RenderShell(scroll, "profile");
    }

    private static TextBox NumberBox(string label, string value)
    {
        return new TextBox
        {
            PlaceholderText = label,
            Text = value,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
    }

    private static string SolveQuadratic(string? aText, string? bText, string? cText)
    {
        if (!TryReadNumber(aText, out var a) || !TryReadNumber(bText, out var b) || !TryReadNumber(cText, out var c))
        {
            return "Nie udało się odczytać współczynników. Sprawdź, czy wpisane wartości są liczbami.";
        }

        if (Math.Abs(a) < 0.0000001)
        {
            return "To nie jest funkcja kwadratowa, ponieważ współczynnik a jest równy 0.";
        }

        var delta = b * b - 4 * a * c;
        var p = -b / (2 * a);
        var q = -delta / (4 * a);

        if (delta > 0)
        {
            var sqrt = Math.Sqrt(delta);
            var x1 = (-b - sqrt) / (2 * a);
            var x2 = (-b + sqrt) / (2 * a);
            return $"Delta = {Format(delta)}\nMiejsca zerowe: x₁ = {Format(x1)}, x₂ = {Format(x2)}\nWierzchołek: P = ({Format(p)}, {Format(q)})";
        }

        if (Math.Abs(delta) < 0.0000001)
        {
            var x0 = -b / (2 * a);
            return $"Delta = 0\nJedno miejsce zerowe: x₀ = {Format(x0)}\nWierzchołek: P = ({Format(p)}, {Format(q)})";
        }

        return $"Delta = {Format(delta)}\nBrak miejsc zerowych w zbiorze liczb rzeczywistych.\nWierzchołek: P = ({Format(p)}, {Format(q)})";
    }

    private static bool TryReadNumber(string? text, out double value)
    {
        var normalized = (text ?? string.Empty).Trim().Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private static string Format(double value)
    {
        return value.ToString("0.###", CultureInfo.InvariantCulture);
    }

    private static ScrollViewer PageScroll()
    {
        return new ScrollViewer
        {
            Padding = new Avalonia.Thickness(30),
            Background = Brush("#F7F4ED")
        };
    }

    private static Control PageTitle(string title, string subtitle)
    {
        var stack = new StackPanel { Spacing = 6 };
        stack.Children.Add(new TextBlock { Text = title, Classes = { "h1" } });
        stack.Children.Add(new TextBlock { Text = subtitle, Classes = { "muted" } });
        return stack;
    }

    private static Control InfoBand(string title, string body)
    {
        var card = Card(new Avalonia.Thickness(18), "#FFFDF8");
        var stack = new StackPanel { Spacing = 4 };
        stack.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 18,
            FontWeight = FontWeight.SemiBold,
            Foreground = Brush("#17202A")
        });
        stack.Children.Add(new TextBlock
        {
            Text = body,
            Classes = { "muted" }
        });
        card.Child = stack;
        return card;
    }

    private static Border Card(Avalonia.Thickness padding, string background)
    {
        return new Border
        {
            Background = Brush(background),
            BorderBrush = Brush("#E1DFD6"),
            BorderThickness = new Avalonia.Thickness(1),
            CornerRadius = new Avalonia.CornerRadius(8),
            Padding = padding
        };
    }

    private static Control Separator()
    {
        return new Border
        {
            Height = 1,
            Background = Brush("#E1DFD6"),
            Margin = new Avalonia.Thickness(0, 8)
        };
    }

    private static Image AssetImage(string assetPath, double width, double? height = null)
    {
        return new Image
        {
            Source = LoadBitmap(assetPath),
            Width = width,
            Height = height ?? width,
            Stretch = Stretch.Uniform,
            HorizontalAlignment = HorizontalAlignment.Left
        };
    }

    private static Bitmap LoadBitmap(string assetPath)
    {
        var uri = new Uri($"avares://Abituria/{assetPath}");
        return new Bitmap(AssetLoader.Open(uri));
    }

    private static IBrush Brush(string color)
    {
        return new SolidColorBrush(Color.Parse(color));
    }

    private static List<FormulaItem> BuildFormulas()
    {
        return new List<FormulaItem>
        {
            new("Funkcja kwadratowa", "Delta, postać kanoniczna i miejsca zerowe są najczęściej używane przy analizie paraboli.", "Δ = b² - 4ac, x = (-b ± √Δ) / 2a", "img/w10.png"),
            new("Ciąg arytmetyczny", "Stała różnica między kolejnymi wyrazami pozwala szybko znaleźć dowolny wyraz i sumę.", "aₙ = a₁ + (n - 1)r, Sₙ = n(a₁ + aₙ) / 2", "img/w11a.png"),
            new("Ciąg geometryczny", "Kolejne wyrazy powstają przez mnożenie przez stały iloraz.", "aₙ = a₁qⁿ⁻¹, Sₙ = a₁(1 - qⁿ) / (1 - q)", "img/w12a.png"),
            new("Trygonometria", "Wartości sinusa, cosinusa i tangensa opisują zależności w trójkątach prostokątnych.", "sin²α + cos²α = 1, tg α = sin α / cos α", "img/w17a.png"),
            new("Prawdopodobieństwo", "W modelu klasycznym liczymy korzystne wyniki i dzielimy przez wszystkie możliwe.", "P(A) = |A| / |Ω|", null),
            new("Geometria analityczna", "Odległość punktów i środek odcinka pomagają pracować na układzie współrzędnych.", "d = √((x₂-x₁)² + (y₂-y₁)²)", "img/w18a.png")
        };
    }

    private static List<TaskImage> BuildTaskImages()
    {
        var names = new[] { "9", "12", "17", "18", "19", "20", "24", "32", "33" };
        return names
            .Select(name => new TaskImage(
                $"Zadanie {name}",
                $"img/mp21z{name}.png",
                "Otwórz zadanie, rozwiąż je na kartce, a kalkulator wykorzystaj tylko do sprawdzenia rachunków."))
            .ToList();
    }

    private sealed record FormulaItem(string Title, string Description, string Example, string? Asset);

    private sealed record TaskImage(string Title, string Asset, string Description);
}

internal sealed class ProfileStore
{
    private const int MaxProfiles = 10;

    public ProfileStore()
    {
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Abituria");
        Directory.CreateDirectory(directory);
        FilePath = Path.Combine(directory, "users.txt");
    }

    public string FilePath { get; }

    public List<string> LoadProfiles()
    {
        if (!File.Exists(FilePath))
        {
            File.WriteAllLines(FilePath, new[] { "Maturzysta" });
        }

        var profiles = File.ReadAllLines(FilePath)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(MaxProfiles)
            .ToList();

        if (profiles.Count == 0)
        {
            profiles.Add("Maturzysta");
            File.WriteAllLines(FilePath, profiles);
        }

        return profiles;
    }

    public AddProfileResult AddProfile(string rawName)
    {
        var name = rawName.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return new AddProfileResult(false, null, "Wpisz nazwę profilu.");
        }

        if (name.Length > 15)
        {
            return new AddProfileResult(false, null, "Nazwa profilu może mieć maksymalnie 15 znaków.");
        }

        var profiles = LoadProfiles();
        if (profiles.Count >= MaxProfiles)
        {
            return new AddProfileResult(false, null, "Możesz mieć maksymalnie 10 profili.");
        }

        if (profiles.Contains(name, StringComparer.OrdinalIgnoreCase))
        {
            return new AddProfileResult(false, null, "Taki profil już istnieje.");
        }

        profiles.Add(name);
        File.WriteAllLines(FilePath, profiles);
        return new AddProfileResult(true, name, "Profil został dodany.");
    }
}

internal sealed record AddProfileResult(bool Success, string? ProfileName, string Message);
