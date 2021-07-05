using System.Windows;

namespace DemoREDScript
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDY0OTg3QDMxMzkyZTMxMmUzMG5FelNzZG1kRFhpdVptdWk5OStQdFBjZEk5SFZlblBhWmZBamIxNTgzL1k9");
        }
    }
}
