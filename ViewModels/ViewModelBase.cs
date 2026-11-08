using CommunityToolkit.Mvvm.ComponentModel;

namespace WestcoastEducation.ViewModels;

//Basklass för alla ViewModels med gemensam status- och felhantering.</summary>
public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isBusy;

    protected void ShowInfo(string message)
    {
        HasError = false;
        StatusMessage = message;
    }

    protected void ShowError(string message)
    {
        HasError = true;
        StatusMessage = message;
    }

    //Kör ett asynkront jobb och fångar fel så att de visas i statusraden.</summary>
    protected async Task RunAsync(Func<Task> action, string? successMessage = null)
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            await action();
            if (successMessage != null)
                ShowInfo(successMessage);
        }
        catch (Exception ex)
        {
            ShowError($"Fel: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

//En sida i appen som laddar sin data första gången den visas.</summary>
public abstract class PageViewModelBase : ViewModelBase
{
    private bool _loaded;

    public abstract string Title { get; }

    public async Task EnsureLoadedAsync()
    {
        if (_loaded)
            return;
        _loaded = true;
        await LoadAsync();
    }

    protected abstract Task LoadAsync();
}
