using Avalonia.Controls;
using StudentManager.ViewModels;

namespace _04_StudentManager;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new StudentViewModel();
    }
}