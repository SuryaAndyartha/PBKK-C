using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using StudentRegistration.Repositories;

namespace StudentRegistration;

public partial class MainWindow : Window
{
    private readonly StudentRepository _studentRepository;

    public MainWindow()
    {
        InitializeComponent();
        _studentRepository = new StudentRepository();
        LoadStudents();
    }

    private void LoadStudents()
    {
        try
        {
            StudentDataGrid.ItemsSource = _studentRepository.GetAll();
            TotalTextBlock.Text =
                $"Total Mahasiswa: {_studentRepository.GetTotalStudents()}";
            ErrorTextBlock.Text = "";
        }
        catch (Exception ex)
        {
            ErrorTextBlock.Text = $"Database Error: {ex.Message}";
        }
    }

    private void RefreshButton_Click(object? sender, RoutedEventArgs e)
    {
        LoadStudents();
    }
}
