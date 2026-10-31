// K07AsyncCommand/DownloadViewModel.cs
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace K07AsyncCommand;

/// <summary>
/// Komenda asynchroniczna: metoda zwraca Task, generator tworzy IAsyncRelayCommand.
/// Podczas wykonywania komenda sama blokuje przycisk, a IsBusy steruje paskiem postępu.
/// </summary>
public partial class DownloadViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial string Result { get; set; } = "Jeszcze nic nie pobrano.";

    [ObservableProperty]
    public partial int Clicks { get; set; }

    // Końcówka "Async" jest obcinana: powstaje DownloadCommand
    [RelayCommand(IncludeCancelCommand = true)]
    private async Task DownloadAsync(CancellationToken token)
    {
        IsBusy = true;
        try
        {
            // Udajemy wolne zapytanie do internetu. Okno pozostaje responsywne,
            // bo await oddaje wątek UI na czas czekania.
            await Task.Delay(TimeSpan.FromSeconds(3), token);
            Result = $"Pobrano o {DateTime.Now:HH:mm:ss}";
        }
        catch (OperationCanceledException)
        {
            Result = $"Anulowano";
        }
        finally
        {
            IsBusy = false;   // także po błędzie – pasek znika, przycisk wraca
        }
    }

    // Zwykła komenda do sprawdzenia, że okno żyje w trakcie pobierania
    [RelayCommand]
    private void Click() => Clicks++;
}