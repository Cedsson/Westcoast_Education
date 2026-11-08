using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using WestcoastEducation.Services;
using WestcoastEducation.ViewModels;
using WestcoastEducation.Views;

namespace WestcoastEducation;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            IRestDbService db = new RestDbService(() => RestDbSettings.Load());
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(db)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
