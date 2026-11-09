using System;
using Microsoft.UI.Xaml;

namespace Calculator;

public sealed partial class MainWindow : Window
{
    private const double DefaultWindowSize = 520;

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");

        RootFrame.Navigate(typeof(MainPage));
        RootFrame.Loaded += RootFrame_Loaded;
    }

    private void RootFrame_Loaded(object sender, RoutedEventArgs e)
    {
        var scale = RootFrame.XamlRoot.RasterizationScale;

        AppWindow.Resize(new Windows.Graphics.SizeInt32(400, 400));
    }
}
