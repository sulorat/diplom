using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

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
            Hide();
        }
        else
        {
            var messageBoxStandardWindow = MessageBoxManager
                .GetMessageBoxStandard("Ошибка", "Неправильный пароль или логин",ButtonEnum.Ok);
            await messageBoxStandardWindow.ShowAsync();
        }
    }

    private void GuestButtonClick(object? sender, RoutedEventArgs e)
    {
        new EquipmentWindow().Show();
        Hide();
    }
}