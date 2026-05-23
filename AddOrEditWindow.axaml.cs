using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using diplom.Models;
using MsBox.Avalonia;

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

        WorkTextBox.Text = _equip.Workhours.ToString();
        CapasityTextBox.Text = _equip.Productioncapacity.ToString();
        NameTextBox.Text = _equip.Name;
        PlaceTextBox.Text = _equip.Place;
        LastDatePicker.SelectedDate = equip.Dateoflastcheck.HasValue ? new DateTimeOffset(equip.Dateoflastcheck.Value.ToDateTime(TimeOnly.MinValue)): null;
        StatusComboBox.SelectedItem = equip.Status?.Name;

    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (NameTextBox.Text == null || PlaceTextBox.Text == null|| LastDatePicker.SelectedDate == null ||
            StatusComboBox.SelectedItem == null|| WorkTextBox.Text == null||CapasityTextBox.Text==null)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Заполните все поля");
            await messageBox.ShowAsync();
            return;
        }
        _equip.Name = NameTextBox.Text;
        _equip.Place = PlaceTextBox.Text;
        int work;
        int capacity;
        if (int.TryParse(WorkTextBox.Text, out work))
        {
            _equip.Workhours = work;
        }
        else
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Рабочие часы должны быть числом");
            await messageBox.ShowAsync();
            return;
        }
        if (int.TryParse(CapasityTextBox.Text, out capacity))
        {
            _equip.Productioncapacity = capacity;
        }
        else
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Рабочие часы должны быть числом");
            await messageBox.ShowAsync();
            return;
        }
        _equip.Dateoflastcheck = DateOnly.FromDateTime(LastDatePicker.SelectedDate.Value.DateTime);
        if (_equip.Dateoflastcheck > DateOnly.FromDateTime(DateTime.Now))
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Нельзя обозначить будущую проверку");
            await messageBox.ShowAsync();
            return;
        }
        _equip.StatusId = StatusComboBox.SelectedIndex + 1;
        Close(_equip);
    }
    
    private async void SelectImageButton_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Choose Equipment Image",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Image Files", Extensions = { "png", "jpg", "jpeg" } }
            }
        };
        string[] result = await dialog.ShowAsync(this);

        if (result != null && result.Length > 0)
        {
            _equip.Imagepath = result[0];
        }
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}