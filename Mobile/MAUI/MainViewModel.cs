using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MyApp.ViewModels;

// CommunityToolkit.Mvvm source generators write the boilerplate INotifyPropertyChanged
// and ICommand wiring at compile time.
public partial class MainViewModel : ObservableObject
{
    private readonly IGreetingService _greetings;

    public MainViewModel(IGreetingService greetings)
    {
        _greetings = greetings;
    }

    [ObservableProperty]
    private string greeting = "Hello, MAUI 10!";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string name = string.Empty;

    public bool IsNotBusy => !IsBusy;

    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task RefreshAsync(CancellationToken ct)
    {
        try
        {
            IsBusy = true;
            Greeting = await _greetings.GetGreetingAsync(Name, ct);
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public interface IGreetingService
{
    Task<string> GetGreetingAsync(string name, CancellationToken ct = default);
}
