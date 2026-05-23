using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using diplom.Models;
using MsBox.Avalonia;

namespace diplom;

public partial class MovementWindow : Window
{
    private List<Equipment> _equipments { get; set; }
    private List<Staff> _staff { get; set; }
    private int type;
    private List<Movementtype> _types { get; set; }
    private Equipment _selectedEquip;
    private Staff _selectedStaff;
    private Movementtype _selectedType;
    
    public MovementWindow()
    {
        InitializeComponent();

        using (var dbcontext = new DiplomContext())
        {
            _equipments = dbcontext.Equipments.Where(e => e.Isdeleted==false||e.Isdeleted==null).ToList();
            _staff = dbcontext.Staff.ToList();
            _types = dbcontext.Movementtypes.ToList();
        }

        EquipComboBox.ItemsSource = _equipments.Where(e=>e.Isdeleted==false||e.Isdeleted==null).Select(e => e.Name);
        TypeComboBox.ItemsSource = _types.Select(e => e.Name);
        StaffComboBox.ItemsSource = _staff.Select(e => e.Name);
        
    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        using (var dbcontext = new DiplomContext())
        {
            _selectedEquip = _equipments.FirstOrDefault(e => e.Name == EquipComboBox.SelectedItem.ToString());
            _selectedStaff = _staff.FirstOrDefault(e => e.Name == StaffComboBox.SelectedItem.ToString());
            _selectedType = _types.FirstOrDefault(e => e.Name == TypeComboBox.SelectedItem.ToString());
            if (_selectedEquip == null || _selectedStaff == null ||
                _selectedType == null || ReasonTextBox.Text == null) 
            {
                var messageBox = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Заполните поля");
                await messageBox.ShowAsync();
                return;
            }
            var equipment = dbcontext.Equipments.FirstOrDefault(e => e.Id == _selectedEquip.Id);
            Equipmentmovement move = new Equipmentmovement();
            switch (type)
            {
                case 0:
                    if (ToTextBox.Text != null )
                    {
                        move.Fromplace = equipment.Place;
                        equipment.Place = ToTextBox.Text;
                        move.EquipmentId = _selectedEquip.Id;
                        move.Toplace = ToTextBox.Text;
                        move.Reason = ReasonTextBox.Text;
                        move.Responsibleperson = _selectedStaff.Id;
                        move.TypeId = _selectedType.Id;
                        dbcontext.SaveChanges();
                    }
                    else
                    {
                        var messageBox = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Заполните поля");
                        await messageBox.ShowAsync();
                        return;
                    }
                    break;
                case 1:
                    equipment.Isdeleted = true;
                    move.EquipmentId = _selectedEquip.Id;
                    move.Reason = ReasonTextBox.Text;
                    move.Responsibleperson = _selectedStaff.Id;
                    move.TypeId = _selectedType.Id;
                    dbcontext.SaveChanges();
                    break;
                case 2:
                    if (FromDatePicker.SelectedDate < ToDatePicker.SelectedDate)
                    {
                        var messageBox = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Заполните даты корректно");
                        await messageBox.ShowAsync();
                        return;
                    }
                    else
                    {
                        move.EquipmentId = _selectedEquip.Id;
                        move.Reason = ReasonTextBox.Text;
                        move.Fromdate = DateOnly.FromDateTime(FromDatePicker.SelectedDate.Value.DateTime);
                        move.Todate = DateOnly.FromDateTime(ToDatePicker.SelectedDate.Value.DateTime);
                        move.Responsibleperson = _selectedStaff.Id;
                        move.TypeId = _selectedType.Id;
                    }
                    break;
            }
            dbcontext.Equipmentmovements.Add(move);
            dbcontext.SaveChanges();
            
        }

        var win = new EquipmentWindow(1);
        win.Show();
        Hide();
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var win = new EquipmentWindow(1);
        win.Show();
        Hide();
    }

    private void TypeComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        type = (sender as ComboBox).SelectedIndex;
        ToTextBox.IsVisible = type == 0;
        ToPlaceTextBlock.IsVisible = type == 0;
        ToDatePicker.IsVisible = type == 2;
        ToTextBlock.IsVisible = type == 2;
        FromDatePicker.IsVisible = type == 2;
        FromTextBlock.IsVisible = type == 2;
    }

    
    
    private void HistoryButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var win = new HistoryMovementWindow();
        win.Show();
        Hide();
    }
}