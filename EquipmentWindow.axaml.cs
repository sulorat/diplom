using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using diplom.Models;
using Microsoft.EntityFrameworkCore;

namespace diplom;

public partial class EquipmentWindow : Window
{
    public class EquipmentPresenter :Equipment
    {
        public Bitmap? ImagePath { get; set; }
        public string EquipName
        {
            get => string.Format("Название: {0}",Name);
        }
        public DateOnly? EquipDateOfLastCheck
        {
            get => Dateoflastcheck;
        }
        public string  EquipDateOfLastCheckString
        {
            get => string.Format("Дата последней проверки: {0}", Dateoflastcheck);
        }
        public string EquipPlace
        {
            get => string.Format("Местоположение: {0}", Place);
        }
        public string EquipStatus
        {
            get => string.Format("статус: {0}", Status?.Name);
        }
        
    }
    private List<EquipmentPresenter> _equipments { get; set; }
    private string searchWord;
    private int countEquip;
    private int role_id;
    private EquipmentPresenter _selectedItem;
    private ObservableCollection<EquipmentPresenter> _equipmentDisplay { get; set; }
    
    public EquipmentWindow()
    {
        InitializeComponent();

        using (var dbcontext = new DiplomContext())
        {
            _equipments = dbcontext.Equipments.Include(e=>e.Status).Select(e => new EquipmentPresenter
            {
                Name = e.Name,
                Status = e.Status,
                StatusId = e.StatusId,
                Place = e.Place,
                Dateoflastcheck = e.Dateoflastcheck,
                Id = e.Id
            }).ToList();
        }
        _equipmentDisplay = new ObservableCollection<EquipmentPresenter>(_equipments);
        EquipListBox.ItemsSource = _equipmentDisplay;

    }
    public EquipmentWindow( int role)
    {
        InitializeComponent();

        using (var dbcontext = new DiplomContext())
        {
            _equipments = dbcontext.Equipments.Include(e=>e.Status).Select(e => new EquipmentPresenter
                {
                    Name = e.Name,
                    Status = e.Status,
                    StatusId = e.StatusId,
                    Place = e.Place,
                    Dateoflastcheck = e.Dateoflastcheck,
                    Id = e.Id
                }).ToList();
        }

        role_id = role;
        _equipmentDisplay = new ObservableCollection<EquipmentPresenter>(_equipments);
        EquipListBox.ItemsSource = _equipmentDisplay;
        AddButton.IsVisible = true;
        EventButton.IsVisible = true;
    }

    private void DisplayEquip()
    {
        List<EquipmentPresenter> displayEquipList = new List<EquipmentPresenter>(_equipments);

        if(!string.IsNullOrEmpty(searchWord))
        {
            displayEquipList = displayEquipList.Where(e => e.EquipName.Contains(searchWord)||e.Place.Contains(searchWord)||e.Status.Name.Contains(searchWord))
                .ToList();
        }
        
        if (_equipmentDisplay.Count != null)
        {
            _equipmentDisplay.Clear();
        }
        foreach (var e in displayEquipList)
        {
            _equipmentDisplay.Add(e);
        }

        EquipCountTextBlock.Text = string.Format(" Количество оборудования: {0}",_equipmentDisplay.Count.ToString());
    }


    private void SearchTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        searchWord = (sender as TextBox).Text;
        DisplayEquip();
    }

    private async void AddButtonClick(object? sender, RoutedEventArgs e)
    {
        var addWindow = new AddOrEditWindow();
        var result = await addWindow.ShowDialog<EquipmentPresenter>(this);
        if (result != null)
        {
            
            await using (var dbcontext = new DiplomContext())
            {
                var equipment = new Equipment()
                {
                    Name = result.Name,
                    Place = result.Place,
                    StatusId = result.StatusId,
                    Dateoflastcheck =  result.Dateoflastcheck,
                };
                result.Status = dbcontext.Statuses.FirstOrDefault(s => s.Id == result.StatusId);
                dbcontext.Equipments.Add(equipment);
                dbcontext.SaveChanges();
                _equipments.Add(result);
            }
        }
        DisplayEquip();
    }
    private async void EventButtonClick(object? sender, RoutedEventArgs e)
    {
        new EventWindow().Show();
        Hide();
    }
    private async void EditButtonClick(object? sender, RoutedEventArgs e)
    {
        var editWindow = new AddOrEditWindow(_selectedItem);
        var result = await editWindow.ShowDialog<EquipmentPresenter>(this);
        using (var dbcontext = new DiplomContext())
        {
            if (result != null)
            {
                _selectedItem.Name = result.Name;
                _selectedItem.StatusId = result.StatusId;
                _selectedItem.Place = result.Place;
                _selectedItem.Dateoflastcheck = result.Dateoflastcheck;
                var equipment = dbcontext.Equipments.FirstOrDefault(e => e.Id == _selectedItem.Id);
                if (equipment != null)
                {
                    equipment.Name = result.Name;
                    equipment.Place = result.Place;
                    equipment.StatusId = result.StatusId;
                    equipment.Dateoflastcheck = result.Dateoflastcheck;
                    _selectedItem.Status = dbcontext.Statuses.FirstOrDefault(s => s.Id == result.StatusId);
                    dbcontext.SaveChanges();
                }
            }
        }
        DisplayEquip();
    }
    private void DeleteButtonClick(object? sender, RoutedEventArgs e)
    {
        _equipments.Remove(_selectedItem);
        using (var dbcontext = new DiplomContext())
        {
            var equip = dbcontext.Equipments.FirstOrDefault(e => e.Id == _selectedItem.Id);
            if (equip != null)
            {
                dbcontext.Equipments.Remove(equip);
                dbcontext.SaveChanges();
            }
            
        }
        DisplayEquip();
    }

    private void EquipListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _selectedItem = (EquipmentPresenter)(sender as ListBox).SelectedItem;
        if (role_id == 1)
        {
            DeleteButton.IsVisible = true;
            EditButton.IsVisible = true;
        }
    }


    private void BackButtonClick(object? sender, RoutedEventArgs e)
    {
        var win = new MainWindow();
        win.Show();
        Hide();
    }
}