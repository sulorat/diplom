using System;
using System.Linq;
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
            var statusNames = dbcontext.Statuses.Select(s => s.Name).ToList();
            StatusComboBox.ItemsSource = statusNames;
        }
    }
    public AddOrEditWindow(EquipmentWindow.EquipmentPresenter equip)
    {
        InitializeComponent();
        _equip = equip;
        TitleTextBlock.Text = "Окно редактирования";
        using (var dbcontext = new DiplomContext())
            
        {
            var statusNames = dbcontext.Statuses.Select(s => s.Name).ToList();
            StatusComboBox.ItemsSource = statusNames;
        }

        NameTextBox.Text = _equip.Name;
        PlaceTextBox.Text = _equip.Place;
        LastDatePicker.SelectedDate = equip.Dateoflastcheck.HasValue ? new DateTimeOffset(equip.Dateoflastcheck.Value.ToDateTime(TimeOnly.MinValue)): null;
        StatusComboBox.SelectedItem = equip.Status?.Name;

    }

    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _equip.Name = NameTextBox.Text;
        _equip.Place = PlaceTextBox.Text;
        _equip.Dateoflastcheck = DateOnly.FromDateTime(LastDatePicker.SelectedDate.Value.DateTime);
        _equip.StatusId = StatusComboBox.SelectedIndex + 1;
        Close(_equip);
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}