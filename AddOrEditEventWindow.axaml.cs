using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using diplom.Models;

namespace diplom;

public partial class AddOrEditEventWindow : Window
{
    private EventWindow.EventPresenter _eventPresenter = new EventWindow.EventPresenter();
    private List<Equipment> equipValues { get; set; }
    private List<Eventstatus> statuses { get; set; }
    private Equipment _selectedEquip;
    public AddOrEditEventWindow()
    {
        InitializeComponent();
        TitleTextBlock.Text = "Окно добавления";
        using (var dbcontext = new DiplomContext())
        {
            statuses = dbcontext.Eventstatuses.ToList();
            equipValues = dbcontext.Equipments.Where(e=>e.Isdeleted!=true).ToList();
        }
        StatusComboBox.ItemsSource = statuses.Select(s=>s.Description);
        EquipComboBox.ItemsSource = equipValues.Select(e => e.Name);
    }
    public AddOrEditEventWindow(EventWindow.EventPresenter _event)
    {
        InitializeComponent();

        using (var dbcontext = new DiplomContext())
        {
            statuses = dbcontext.Eventstatuses.ToList();
            equipValues = dbcontext.Equipments.ToList();
        }

        DescriptionTextBox.Text = _event.Description;
        StatusComboBox.ItemsSource = statuses.Select(s=>s.Description);
        EquipComboBox.ItemsSource = equipValues.Select(e => e.Name);
        StatusComboBox.SelectedItem = _event.Eventstatus.Description;
        EquipComboBox.SelectedItem = _event.Equipment.Name;
        _eventPresenter = _event;
        TitleTextBlock.Text = "Окно редактирования";
        DateCheckPicker.SelectedDate = new DateTimeOffset(_eventPresenter.Dateof.ToDateTime(TimeOnly.MinValue));
        
    }

    
    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DescriptionTextBox.Text == null || DateCheckPicker == null||
            StatusComboBox.SelectedItem == null ||EquipComboBox == null)
        {
            ErrorTextBlock.Text = "Заполните все поля";
            await Task.Delay(4000);
            ErrorTextBlock.Text = "";
            return;
        }
        _eventPresenter.Description = DescriptionTextBox.Text;
        _eventPresenter.EventstatusId = StatusComboBox.SelectedIndex + 1;
        _selectedEquip = equipValues.FirstOrDefault(e => e.Name == EquipComboBox.SelectedItem.ToString());
        if (_selectedEquip != null)
        {
            _eventPresenter.EquipmentId = _selectedEquip.Id;
        }
        _eventPresenter.Dateof = DateOnly.FromDateTime(DateCheckPicker.SelectedDate.Value.DateTime);
        if (_eventPresenter.Dateof < DateOnly.FromDateTime(DateTime.Now))
        {
            ErrorTextBlock.Text = "Нельзя выбрать дату до сегодняшнего дня";
            await Task.Delay(4000);
            ErrorTextBlock.Text = "";
            return;
        }
        
        Close(_eventPresenter);
        
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}