using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace IdolyPrideLocalizator;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SystemBackdrop = new MicaBackdrop();
    }
}
