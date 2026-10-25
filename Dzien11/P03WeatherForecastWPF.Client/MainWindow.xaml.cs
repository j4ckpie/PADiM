// P03WeatherForecastWPF.Client/MainWindow.xaml.cs
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Configuration;
using P03WeatherForecastWPF.Client.Models;
using P03WeatherForecastWPF.Client.Services;

namespace P03WeatherForecastWPF.Client;

/// <summary>
/// Wersja z dnia 11: logika w code-behind (obsługa zdarzeń w pliku okna).
/// Jutro (dzień 12) przeniesiemy ją do ViewModelu (wzorzec MVVM).
/// </summary>
public partial class MainWindow : Window
{
    private readonly OpenMeteoService _openMeteoService;

    private readonly ObservableCollection<City> _favorites = new();

    private readonly string _favoritesPath =  Path.Combine(AppContext.BaseDirectory, "favorties.json");

    public MainWindow()
    {
        InitializeComponent();

        // Konfiguracja z pliku appsettings.json (pakiet Microsoft.Extensions.Configuration.Json).
        // Plik ma ustawione "Copy to Output Directory", więc leży obok .exe.
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string language = configuration["AppSettings:DefaultLanguage"] ?? "en";

        _openMeteoService = new OpenMeteoService(new HttpClient(), language);
        LoadFavorites();
    }

    private async void btnSearch_Click(object sender, RoutedEventArgs e)
    {
        tbStatus.Text = string.Empty;
        btnSearch.IsEnabled = false;

        try
        {
            City[] cities = await _openMeteoService.GetLocationsAsync(txtCity.Text);
            lbCities.ItemsSource = cities;

            if (cities.Length == 0)
            {
                tbStatus.Text = "Nie znaleziono miasta o takiej nazwie.";
            }
        }
        catch (HttpRequestException ex)
        {
            tbStatus.Text = $"Błąd połączenia: {ex.Message}";
        }
        finally
        {
            btnSearch.IsEnabled = true;
        }
    }

    private async void btnFavorite_Click(object sender, RoutedEventArgs e)
    {
        if(lbCities.SelectedItem is not City selectedCity)
        {
            tbStatus.Text = "Najpierw zaznacz miasto, które chcesz dodać do ulubionych";
            return;
        }

        bool alreadyAdded = _favorites.Any(f =>
        f.Latitude == selectedCity.Latitude && f.Longitude == selectedCity.Longitude);

        if(alreadyAdded)
        {
            tbStatus.Text = $"{selectedCity.Name} jest już w ulubionych!";
            return;
        }

        _favorites.Add(selectedCity);
        SaveFavorties();
        tbStatus.Text = $"Dodano {selectedCity.Name} do ulubionych!";
    }

    private async void lbCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lbCities.SelectedItem is not City selectedCity)
        {
            return;
        }

        tbStatus.Text = string.Empty;

        try
        {
            Weather? weather = await _openMeteoService.GetCurrentWeatherAsync(selectedCity.Latitude, selectedCity.Longitude);

            lblCityName.Content = selectedCity.FullName;

            if (weather is null)
            {
                tbStatus.Text = "Brak danych pogodowych dla tej lokalizacji.";
                return;
            }

            lblTemperature.Content = $"{weather.Temperature:F1} °C";
            lblWind.Content = $"{weather.WindSpeed:F0} km/h";
            lblDescription.Content = weather.Description;
            lblTime.Content = weather.Time.ToString("g");
        }
        catch (HttpRequestException ex)
        {
            tbStatus.Text = $"Błąd połączenia: {ex.Message}";
        }
    }

    private void SaveFavorties()
    {
        string json = JsonSerializer.Serialize(_favorites, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_favoritesPath, json);
    }

    private void LoadFavorites()
    {
        if(!File.Exists(_favoritesPath)) { return; }
        
        string json = File.ReadAllText(_favoritesPath);

        if(string.IsNullOrWhiteSpace(json)) { return; }

        List<City>? loaded = JsonSerializer.Deserialize<List<City>>(json);

        foreach(City city in loaded ?? [])
        {
            _favorites.Add(city);
        }

        lbCities.ItemsSource = _favorites;
    }
}