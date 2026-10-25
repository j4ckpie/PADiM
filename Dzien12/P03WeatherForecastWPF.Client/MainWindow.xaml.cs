// P03WeatherForecastWPF.Client/MainWindow.xaml.cs  (wersja tymczasowa – pełna w sekcji 7)
using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.Options;
using P03WeatherForecastWPF.Client.Configuration;
using P03WeatherForecastWPF.Client.Services;
using P03WeatherForecastWPF.Client.ViewModels.Classic;

namespace P03WeatherForecastWPF.Client;

/// <summary>
/// Wersja tymczasowa (etap 4): prawdziwy serwis, ale wszystko nadal tworzymy ręcznie.
/// W etapie 7 zrobi to za nas kontener Dependency Injection.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Options.Create opakowuje obiekt ustawień w IOptions<T> – tak samo zrobi kontener DI,
        // tylko wartości weźmie z appsettings.json (etap 7). Na razie język na sztywno.
        IOptions<AppSettings> settings = Options.Create(new AppSettings { DefaultLanguage = "pl" });

        DataContext = new MainViewModelClassic(new OpenMeteoService(new HttpClient(), settings));

        // Wersja bez internetu – ViewModel nie widzi różnicy:
        // DataContext = new MainViewModelClassic(new FakeMeteoService());
    }
}