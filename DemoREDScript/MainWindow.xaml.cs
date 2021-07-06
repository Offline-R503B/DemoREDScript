using Syncfusion.Windows.Edit;
using System.Windows;

namespace DemoREDScript
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RedscriptLanguage redscriptLanguage = new(editControl);
            redscriptLanguage.Lexem = Resources["redscriptLexem"] as LexemCollection;
            redscriptLanguage.Formats = Resources["redscriptDefaultFormat"] as FormatsCollection;

            editControl.DocumentLanguage = Languages.Custom;
            editControl.CustomLanguage = redscriptLanguage;
            editControl.EnableOutlining = true;

            editControl.DocumentSource = "sample.reds";
        }
    }
}
