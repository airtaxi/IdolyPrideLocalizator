using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace IdolyPrideLocalizator;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon("Icon.ico");
        SystemBackdrop = new MicaBackdrop();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBar);
    }
}
