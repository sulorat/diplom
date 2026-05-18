using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using diplom.Models;

namespace diplom;

public partial class AddOrEditWindow : Window
{
    private EquipmentWindow.EquipmentPresenter _equip = new();
    public AddOrEditWindow()
    {
        InitializeComponent();
        TitleTextBlock.Text = "Окно добавления";
        
        using (var dbcontext = new DiplomContext())
        {
            var statusNames = dbcontext.Statuses.ToList();
            StatusComboBox.ItemsSource = statusNames.Select(s=>s.Name);
        }
    }
    public AddOrEditWindow(EquipmentWindow.EquipmentPresenter equip)
    {
        InitializeComponent();
        _equip = equip;
        TitleTextBlock.Text = "Окно редактирования";
        using (var dbcontext = new DiplomContext())

        {
            var statusNames = dbcontext.Statuses.ToList();
            StatusComboBox.ItemsSource = statusNames.Select(s=>s.Name);
        }

        NameTextBox.Text = _equip.Name;
        PlaceTextBox.Text = _equip.Place;
        LastDatePicker.SelectedDate = equip.Dateoflastcheck.HasValue ? new DateTimeOffset(equip.Dateoflastcheck.Value.ToDateTime(TimeOnly.MinValue)): null;
        StatusComboBox.SelectedItem = equip.Status?.Name;

    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (NameTextBox.Text == null || PlaceTextBox.Text == null || LastDatePicker.SelectedDate == null ||
            StatusComboBox.SelectedItem == null)
        {
            ErrorTextBlock.Text = "Заполните все поля";
            await Task.Delay(4000);
            ErrorTextBlock.Text = "";
            return;
        }
        _equip.Name = NameTextBox.Text;
        _equip.Place = PlaceTextBox.Text;
        _equip.Dateoflastcheck = DateOnly.FromDateTime(LastDatePicker.SelectedDate.Value.DateTime);
        if (_equip.Dateoflastcheck > DateOnly.FromDateTime(DateTime.Now))
        {
            ErrorTextBlock.Text = "Нельзя обозначить будущую проверку";
            await Task.Delay(4000);
            ErrorTextBlock.Text = "";
            return;
        }
        _equip.StatusId = StatusComboBox.SelectedIndex + 1;
        Close(_equip);
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}