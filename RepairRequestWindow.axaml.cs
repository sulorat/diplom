using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using diplom.Models;

namespace diplom;

public partial class RepairRequestWindow : Window
{
    public class RepairPresenter : Repairrequest
    {
        public string? ImagePath
        {
            get => Equipment.Imagepath;
        }
        
        public Bitmap Image
        {
            get
            {
                if (string.IsNullOrEmpty( Equipment.Imagepath) || !File.Exists( Equipment.Imagepath))
                    return null;

                return new Bitmap( Equipment.Imagepath);
            }
        }

        public string RepairEquip
        {
            get => string.Format("Название: {0}",Equipment.Name);
        }
        public string RepairDescription
        {
            get => string.Format("Описание проблемы: {0}",Description);
        }
        public string? RepairPriority
        {
            get => string.Format("Приоритет: {0}",PriorityNavigation.Name);
        }
        public string? RepairRequestStatus
        {
            get => string.Format("Стасус: {0}",Status.Name);
        }
        public DateOnly? RepairCreatedDate
        {
            get => Createdat;
        }
        public string RepairCreatedDateString
        {
            get => string.Format("Заявка создана: {0}",Createdat);
        }
    }
    
    private List<RepairPresenter> _repairs { get; set; }
    private ObservableCollection<RepairPresenter> _displayRepairs { get; set; }
    private string[] sortValues = ["Все", "Срочно", "Обычная очередь"];
    private RepairPresenter _selectedItem;
    private int sortIndex;
    
    public RepairRequestWindow()
    {
        InitializeComponent();

        using (var dbcontext = new DiplomContext())
        {
            _repairs = dbcontext.Repairrequests.Where(e=>e.Isdeleted!=true).Select(e => new RepairPresenter()
            {
                Id = e.Id,
                EquipmentId = e.EquipmentId,
                Equipment = e.Equipment,
                Description = e.Description,
                Priority = e.Priority,
                StatusId = e.StatusId,
                Createdat = e.Createdat,
                Isdeleted = e.Isdeleted,
                PriorityNavigation = e.PriorityNavigation,
                Status = e.Status
            }).ToList();
        }

        _displayRepairs = new ObservableCollection<RepairPresenter>(_repairs);
        RepairsListBox.ItemsSource = _displayRepairs;
        SortComboBox.ItemsSource = sortValues;
    }

    private void DisplayRepairs()
    {
        List<RepairPresenter> displayList = new List<RepairPresenter>(_repairs);

        switch (sortIndex)
        {
            case 1:
                displayList = displayList.Where(e => e.PriorityNavigation.Id == 1).ToList();
                break;
            case 2:
                displayList = displayList.Where(e => e.PriorityNavigation.Id == 2).ToList();
                break;
        }
        
        if (_displayRepairs.Count > 0)
        {
            _displayRepairs.Clear();
        }
        foreach (var repairs in displayList)
        {
            _displayRepairs.Add(repairs);
        }
    }
    

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var win = new EventWindow();
        win.Show();
        Hide();
    }


    private void RepairsListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _selectedItem = (RepairPresenter)(sender as ListBox).SelectedItem;
        AddButton.IsVisible = true;
        DeleteButton.IsVisible = true;
    }

    private void SortComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        sortIndex = (sender as ComboBox).SelectedIndex;
        DisplayRepairs();
    }

    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        using (var dbcontext = new DiplomContext())
        {
            var repair = dbcontext.Repairrequests.FirstOrDefault(e => e.Id == _selectedItem.Id);
            if (repair != null)
            {
                repair.Isdeleted = true;
                _repairs.Remove(_selectedItem);
                DisplayRepairs();
                dbcontext.SaveChanges();
            } return;
        }
    }

    private void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        using (var dbcontext = new DiplomContext())
        {
           var _event = new Event();
           _event.Description = _selectedItem.Description;
           _event.EquipmentId = _selectedItem.EquipmentId;
           _event.EventstatusId = 2;
           _event.Dateof = _selectedItem.Createdat?? DateOnly.FromDateTime(DateTime.Now);
           var repair = dbcontext.Repairrequests.FirstOrDefault(e => e.Id == _selectedItem.Id);
           if (repair != null)
           {
               repair.Isdeleted = true;
               _repairs.Remove(_selectedItem);
               DisplayRepairs();
               dbcontext.SaveChanges();
           }
           else
           {
               return;
           }
           dbcontext.Events.Add(_event);
           dbcontext.SaveChanges();
        }
        
    }
}