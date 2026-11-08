using System;
using System.Numerics;

namespace Calculator.Services;

public sealed class NumberBaseConverter
{
    private const string Digits = "0123456789ABCDEF";

    public bool TryParse(string? text, int numberBase, out BigInteger value)
    {
        value = BigInteger.Zero;

        if (numberBase is < 2 or > 16 || string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var span = text.AsSpan().Trim();
        var isNegative = false;

        if (span[0] is '-' or '+')
        {
            isNegative = span[0] == '-';
            span = span[1..];
        }

        if (span.IsEmpty)
        {
            return false;
        }

        foreach (var character in span)
        {
            var digit = GetDigitValue(character);
            if (digit < 0 || digit >= numberBase)
            {
                value = BigInteger.Zero;
                return false;
            }

            value = (value * numberBase) + digit;
        }

        if (isNegative)
        {
            value = BigInteger.Negate(value);
        }

        return true;
    }

    public string Format(BigInteger value, int numberBase)
    {
        if (numberBase is < 2 or > 16)
        {
            throw new ArgumentOutOfRangeException(nameof(numberBase), "Основание системы должно быть от 2 до 16.");
        }

        if (value.IsZero)
        {
            return "0";
        }

        var isNegative = value.Sign < 0;
        value = BigInteger.Abs(value);

        Span<char> buffer = stackalloc char[128];
        var position = buffer.Length;

        while (value > BigInteger.Zero)
        {
            value = BigInteger.DivRem(value, numberBase, out var remainder);
            buffer[--position] = Digits[(int)remainder];
        }

        var result = new string(buffer[position..]);
        return isNegative ? $"-{result}" : result;
    }

    private static int GetDigitValue(char character)
    {
        if (character is >= '0' and <= '9')
        {
            return character - '0';
        }

        character = char.ToUpperInvariant(character);

        if (character is >= 'A' and <= 'F')
        {
            return character - 'A' + 10;
        }

        return -1;
    }
}
