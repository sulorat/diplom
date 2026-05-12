using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace diplom;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void LogButtonClick(object? sender, RoutedEventArgs e)
    {
        if (LogTextBox.Text == "admin" && PassTextBox.Text == "admin")
        {
            new EquipmentWindow(1).Show();    
        }
        else
        {
            ErrorTextBlock.Text = "Неправильный пароль или логин";
            await Task.Delay(3000);
            ErrorTextBlock.Text = "";
        }
        Hide();
    }

    private void GuestButtonClick(object? sender, RoutedEventArgs e)
    {
        new EquipmentWindow().Show();
        Hide();
    }
}