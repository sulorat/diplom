using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using diplom.Models;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace diplom;

public partial class EquipmentWindow : Window
{
    public class EquipmentPresenter :Equipment
    {
        public IBrush? EquipColor
        {
            get => this.Workhours > this.Productioncapacity ? Brushes.Brown: null;
        }
        public string? EquipImagePath {get;set;}
        public Bitmap? Image
        {
            get
            {
                if (string.IsNullOrEmpty(Imagepath) || !File.Exists(Imagepath))
                    return null;

                return new Bitmap(Imagepath);
            }
        }
        public string EquipName
        {
            get => string.Format("Название: {0}",Name);
        }
        public string EquipWorkHours
        {
            get => string.Format("Проработанные часы: {0}",Workhours);
        }
        public string EquipCapasity
        {
            get => string.Format("Максимальная производительная мощь: {0}",Productioncapacity);
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
    private int sortIdex;
    private string[] statuses = ["Все", "Работают", "Не работают", "Нужна проверка", "Другое/неизвестно"];
    private EquipmentPresenter _selectedItem;
    private ObservableCollection<EquipmentPresenter> _equipmentDisplay { get; set; }
    
    public EquipmentWindow()
    {
        InitializeComponent();

        using (var dbcontext = new DiplomContext())
        {
            _equipments = dbcontext.Equipments.Include(e=>e.Status).Include(e=>e.Equipmentmovements).Select(e => new EquipmentPresenter
            {
                Name = e.Name,
                Status = e.Status,
                StatusId = e.StatusId,
                Place = e.Place,    
                Workhours = e.Workhours,
                Equipmentmovements = e.Equipmentmovements,
                Productioncapacity = e.Productioncapacity,
                Isdeleted = e.Isdeleted,
                Dateoflastcheck = e.Dateoflastcheck,
                Imagepath = e.Imagepath,
                Id = e.Id
            }).Where(e=>e.Isdeleted==false||e.Isdeleted==null).ToList();
        }
        _equipmentDisplay = new ObservableCollection<EquipmentPresenter>(_equipments);
        EquipListBox.ItemsSource = _equipmentDisplay;

    }
    public EquipmentWindow( int role)
    {
        InitializeComponent();

        using (var dbcontext = new DiplomContext())
        {
            _equipments = dbcontext.Equipments.Include(e=>e.Status).Where(e=>e.Isdeleted==false||e.Isdeleted==null).Select(e => new EquipmentPresenter
                {
                    Name = e.Name,
                    Status = e.Status,
                    StatusId = e.StatusId,
                    Imagepath = e.Imagepath,
                    Workhours = e.Workhours,
                    Isdeleted = e.Isdeleted,
                    Productioncapacity = e.Productioncapacity,
                    Place = e.Place,
                    Dateoflastcheck = e.Dateoflastcheck,
                    Id = e.Id
                }).ToList();
            
        }

        MoveButton.IsVisible = true;
        SortComboBox.IsVisible = true;
        RequestButton.IsVisible = false;
        SortComboBox.ItemsSource = statuses;
        role_id = role;
        _equipmentDisplay = new ObservableCollection<EquipmentPresenter>(_equipments);
        EquipListBox.ItemsSource = _equipmentDisplay;
        HistoryButton.IsVisible = true;
        AddButton.IsVisible = true;
        EventButton.IsVisible = true;
        DisplayEquip();
    }

    private void DisplayEquip()
    {
        List<EquipmentPresenter> displayEquipList = new List<EquipmentPresenter>(_equipments);

        if (_equipments.Any(e => e.EquipColor != null))
        {
            WarningTextBlock.Text = "Одно из оборудований может выйти из строя";
        }
        
        if(!string.IsNullOrEmpty(searchWord))
        {
            displayEquipList = displayEquipList.Where(e => e.EquipName.Contains(searchWord)||e.Place.Contains(searchWord)||e.Status.Name.Contains(searchWord))
                .ToList();
        }
        
        switch (sortIdex)
        {
            case 1:
                displayEquipList = displayEquipList.Where(e => e.StatusId == 1).ToList();
                break;
            case 2:
                displayEquipList = displayEquipList.Where(e => e.StatusId == 2).ToList();
                break;
            case 3:
                displayEquipList = displayEquipList.Where(e => e.StatusId == 3).ToList();
                break;
            case 4:
                displayEquipList = displayEquipList.Where(e => e.StatusId == 4).ToList();
                break;
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
                    Productioncapacity = result.Productioncapacity,
                    Workhours = result.Workhours,
                    StatusId = result.StatusId,
                    Imagepath = result.Imagepath,
                    Dateoflastcheck =  result.Dateoflastcheck,
                };
                result.Id = equipment.Id;
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
                _selectedItem.Productioncapacity = result.Productioncapacity;
                _selectedItem.Workhours = result.Workhours;
                _selectedItem.Isdeleted = result.Isdeleted;
                _selectedItem.Imagepath = result.Imagepath;
                _selectedItem.Dateoflastcheck = result.Dateoflastcheck;
                var equipment = dbcontext.Equipments.FirstOrDefault(e => e.Id == _selectedItem.Id);
                if (equipment != null)
                {
                    equipment.Name = result.Name;
                    equipment.Place = result.Place;
                    equipment.Workhours = result.Workhours;
                    equipment.Productioncapacity = result.Productioncapacity;
                    equipment.StatusId = result.StatusId;
                    equipment.Imagepath = result.Imagepath;
                    equipment.Dateoflastcheck = result.Dateoflastcheck;
                    _selectedItem.Status = dbcontext.Statuses.FirstOrDefault(s => s.Id == result.StatusId);
                    dbcontext.SaveChanges();
                }
            }
        }
        DisplayEquip();
    }
    private async void DeleteButtonClick(object? sender, RoutedEventArgs e)
    {
        
        using (var dbcontext = new DiplomContext())
        {
            var equip = dbcontext.Equipments.FirstOrDefault(e => e.Id == _selectedItem.Id);
            if (equip != null)
            {
                var messageBox = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams()
                {
                    ContentTitle = "Удаление",
                    ContentMessage = "Удалить запись?",
                    ButtonDefinitions = ButtonEnum.YesNo
                });
                var result = await messageBox.ShowAsync();
                if (result== ButtonResult.Yes)
                {
                    var _event = dbcontext.Events.FirstOrDefault(e => e.EquipmentId == _selectedItem.Id);
                    if (_event == null)
                    {
                        _equipments.Remove(_selectedItem);
                        equip.Isdeleted = true;
                        dbcontext.SaveChanges();
                    }
                    else
                    {
                        var messageBoxStandardWindow = MessageBoxManager
                            .GetMessageBoxStandard("Ошибка", "Нельзя удалять оборудование, с которым проводится работа",ButtonEnum.Ok);
                            
                        await messageBoxStandardWindow.ShowAsync();
                        return;
                    }
                }
                else
                {
                    return;
                }
                
            }
            else
            {
                return;
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

    private void SortComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        sortIdex = (sender as ComboBox).SelectedIndex;
        DisplayEquip();
    }

    private void HistoryButton_OnClick(object? sender, RoutedEventArgs e)
    {
        new HistoryEquipWindow().Show();
        Hide();
    }

    private async void MoveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var win = new MovementWindow();
        win.Show();
        Hide();
    }

    private void RequestButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var win = new AddRequestsWindow();
        win.Show();
        Hide();
    }
}