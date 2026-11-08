using System;
using Microsoft.UI.Xaml;

namespace Calculator;

public sealed partial class MainWindow : Window
{
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
        var width = (int)Math.Round(900 * scale);
        var height = (int)Math.Round(760 * scale);

        AppWindow.Resize(new Windows.Graphics.SizeInt32(width, height));
    }
}
