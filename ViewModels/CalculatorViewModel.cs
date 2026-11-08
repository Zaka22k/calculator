using System;
using System.Collections.Generic;
using System.Numerics;
using Calculator.Infrastructure;
using Calculator.Models;
using Calculator.Services;

namespace Calculator.ViewModels;

public sealed class CalculatorViewModel : ObservableObject
{
    private readonly NumberBaseConverter _converter = new();

    private static readonly IReadOnlyList<NumberBaseOption> AvailableBases =
    [
        new("HEX (16)", 16),
        new("DEC (10)", 10),
        new("OCT (8)", 8),
        new("BIN (2)", 2)
    ];

    private int _selectedBase = 10;
    private string _inputText = "0";
    private string _hexValue = "0";
    private string _decimalValue = "0";
    private string _octalValue = "0";
    private string _binaryValue = "0";
    private bool _hasError;
    private string _errorMessage = string.Empty;

    public IReadOnlyList<NumberBaseOption> BaseOptions => AvailableBases;

    public RelayCommand ClearCommand { get; }
    public RelayCommand BackspaceCommand { get; }
    public RelayCommand ToggleSignCommand { get; }

    public int SelectedBase
    {
        get => _selectedBase;
        set
        {
            if (value is not (2 or 8 or 10 or 16) || value == _selectedBase)
            {
                return;
            }

            if (!_converter.TryParse(InputText, _selectedBase, out var currentValue))
            {
                currentValue = BigInteger.Zero;
            }

            if (!SetProperty(ref _selectedBase, value))
            {
                return;
            }

            InputText = _converter.Format(currentValue, _selectedBase);
            UpdateConversions(currentValue);
            ClearError();
        }
    }

    public string InputText
    {
        get => _inputText;
        private set => SetProperty(ref _inputText, value);
    }

    public string HexValue
    {
        get => _hexValue;
        private set => SetProperty(ref _hexValue, value);
    }

    public string DecimalValue
    {
        get => _decimalValue;
        private set => SetProperty(ref _decimalValue, value);
    }

    public string OctalValue
    {
        get => _octalValue;
        private set => SetProperty(ref _octalValue, value);
    }

    public string BinaryValue
    {
        get => _binaryValue;
        private set => SetProperty(ref _binaryValue, value);
    }

    public bool HasError
    {
        get => _hasError;
        private set => SetProperty(ref _hasError, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public CalculatorViewModel()
    {
        ClearCommand = new RelayCommand(Clear);
        BackspaceCommand = new RelayCommand(Backspace, CanBackspace);
        ToggleSignCommand = new RelayCommand(ToggleSign, CanToggleSign);

        UpdateConversions(BigInteger.Zero);
    }

    public void AppendDigit(char character)
    {
        character = char.ToUpperInvariant(character);
        var digitValue = GetDigitValue(character);

        if (digitValue < 0)
        {
            return;
        }

        if (digitValue >= SelectedBase)
        {
            HasError = true;
            ErrorMessage = $"Цифра «{character}» недопустима в системе счисления с основанием {SelectedBase}.";
            return;
        }

        ClearError();

        var nextText = InputText == "0"
            ? character.ToString()
            : InputText + character;

        if (_converter.TryParse(nextText, SelectedBase, out var value))
        {
            InputText = nextText;
            UpdateConversions(value);
            NotifyInputCommands();
        }
    }

    private void Clear()
    {
        InputText = "0";
        ClearError();
        UpdateConversions(BigInteger.Zero);
        NotifyInputCommands();
    }

    private void Backspace()
    {
        if (!CanBackspace())
        {
            return;
        }

        var nextText = InputText[..^1];
        if (nextText.Length == 0 || nextText == "-")
        {
            nextText = "0";
        }

        if (_converter.TryParse(nextText, SelectedBase, out var value))
        {
            InputText = nextText;
            ClearError();
            UpdateConversions(value);
            NotifyInputCommands();
        }
    }

    private bool CanBackspace()
    {
        return InputText.Length > 1 || (InputText.Length == 1 && InputText != "0");
    }

    private void ToggleSign()
    {
        if (!_converter.TryParse(InputText, SelectedBase, out var value) || value.IsZero)
        {
            return;
        }

        value = BigInteger.Negate(value);
        InputText = _converter.Format(value, SelectedBase);
        ClearError();
        UpdateConversions(value);
        NotifyInputCommands();
    }

    private bool CanToggleSign()
    {
        return InputText != "0";
    }

    private void UpdateConversions(BigInteger value)
    {
        HexValue = _converter.Format(value, 16);
        DecimalValue = _converter.Format(value, 10);
        OctalValue = _converter.Format(value, 8);
        BinaryValue = _converter.Format(value, 2);
    }

    private void ClearError()
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }

    private void NotifyInputCommands()
    {
        BackspaceCommand.NotifyCanExecuteChanged();
        ToggleSignCommand.NotifyCanExecuteChanged();
    }

    private static int GetDigitValue(char character)
    {
        if (character is >= '0' and <= '9')
        {
            return character - '0';
        }

        if (character is >= 'A' and <= 'F')
        {
            return character - 'A' + 10;
        }

        return -1;
    }
}
