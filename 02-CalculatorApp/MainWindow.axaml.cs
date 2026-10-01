using System;
using System.Data;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace CalculatorApp;

public partial class MainWindow : Window
{
    string expression = "";
    bool justEvaluated = false;

    private const string Operators = "+−×÷";

    public MainWindow()
    {
        InitializeComponent();
    }

    private void NumberButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            if (justEvaluated)
            {
                expression = "";
                justEvaluated = false;
            }

            string number = button.Content?.ToString() ?? "";
            expression += number;
            UpdateDisplay();
        }
    }

    private void OperatorButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (sender is Button button && !string.IsNullOrEmpty(expression))
        {
            justEvaluated = false;
            string op = button.Content?.ToString() ?? "";

            // Kalau operator ditekan dua kali berturut-turut, operator lama diganti
            string trimmed = expression.TrimEnd();
            if (trimmed.Length > 0 && Operators.Contains(trimmed[^1]))
                trimmed = trimmed[..^1].TrimEnd();

            expression = $"{trimmed} {op} ";
            UpdateDisplay();
        }
    }

    private async void btnEquals_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(expression)) return;

        try
        {
            string evaluable = expression
                .Replace('÷', '/')
                .Replace('×', '*')
                .Replace('−', '-');

            object? computed = new DataTable().Compute(evaluable, null);
            double result = Convert.ToDouble(computed);

            if (double.IsInfinity(result))
                throw new DivideByZeroException("Tidak dapat membagi dengan nol.");

            expression = result.ToString();
            justEvaluated = true;
            UpdateDisplay();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex.Message);
            expression = "";
            justEvaluated = false;
            UpdateDisplay();
        }
    }

    private void btnClear_Click(
        object? sender,
        RoutedEventArgs e)
    {
        expression = "";
        justEvaluated = false;
        UpdateDisplay();
    }

    private void btnDecimal_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (justEvaluated)
        {
            expression = "";
            justEvaluated = false;
        }

        expression += ".";
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        txtDisplay.Text = string.IsNullOrEmpty(expression) ? "0" : expression;
    }

    private async Task ShowErrorAsync(string message)
    {
        var dialog = new Window
        {
            Title = "Error",
            Width = 300,
            Height = 180,
            WindowStartupLocation =
                WindowStartupLocation.CenterOwner
        };

        var panel = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 15
        };

        panel.Children.Add(new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap
        });

        var button = new Button
        {
            Content = "OK",
            HorizontalAlignment =
                HorizontalAlignment.Center
        };

        button.Click += (_, _) => dialog.Close();

        panel.Children.Add(button);

        dialog.Content = panel;

        await dialog.ShowDialog(this);
    }
}