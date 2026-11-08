using Calculator.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace Calculator;

public sealed partial class MainPage : Page
{
    private const int OemMinusVirtualKey = 0xBD;

    public CalculatorViewModel ViewModel { get; }

    public MainPage()
    {
        InitializeComponent();

        ViewModel = new CalculatorViewModel();
        DataContext = ViewModel;
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        BaseList.SelectedIndex = ViewModel.SelectedBase switch
        {
            16 => 0,
            10 => 1,
            8 => 2,
            2 => 3,
            _ => 1
        };

        InputDisplay.Focus(FocusState.Programmatic);
    }

    private void BaseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListView list
            || list.SelectedItem is not ListViewItem item
            || !int.TryParse(item.Tag?.ToString(), out var numberBase))
        {
            return;
        }

        ViewModel.SelectedBase = numberBase;
        InputDisplay.Focus(FocusState.Programmatic);
    }

    private void MainPage_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Back)
        {
            ViewModel.BackspaceCommand.Execute(null);
            e.Handled = true;
            return;
        }

        if (e.Key == VirtualKey.Escape)
        {
            ViewModel.ClearCommand.Execute(null);
            e.Handled = true;
            return;
        }

        if (IsMinusKey(e.Key))
        {
            ViewModel.ToggleSignCommand.Execute(null);
            e.Handled = true;
            return;
        }

        var digit = GetDigit(e.Key);
        if (digit.HasValue)
        {
            ViewModel.AppendDigit(digit.Value);
            e.Handled = true;
        }
    }

    private static bool IsMinusKey(VirtualKey key)
    {
        // VK_OEM_MINUS (0xBD) is the physical keyboard minus key.
        // VirtualKey.Subtract represents the numeric keypad minus key.
        return (int)key == OemMinusVirtualKey || key == VirtualKey.Subtract;
    }

    private static char? GetDigit(VirtualKey key)
    {
        var keyCode = (int)key;

        if (keyCode >= (int)VirtualKey.Number0 && keyCode <= (int)VirtualKey.Number9)
        {
            return (char)('0' + keyCode - (int)VirtualKey.Number0);
        }

        if (keyCode >= (int)VirtualKey.NumberPad0 && keyCode <= (int)VirtualKey.NumberPad9)
        {
            return (char)('0' + keyCode - (int)VirtualKey.NumberPad0);
        }

        if (keyCode >= (int)VirtualKey.A && keyCode <= (int)VirtualKey.F)
        {
            return (char)('A' + keyCode - (int)VirtualKey.A);
        }

        return null;
    }
}
