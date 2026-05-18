using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using diplom.Models;
using Microsoft.EntityFrameworkCore;

namespace diplom;

public partial class EventWindow : Window
{
    public class EventPresenter: Event
    {
        public string EventEquip
        {
            get => string.Format("Оборудование: {0}",Equipment?.Name ?? "");
        }
        public int EventId
        {
            get => Id;
        }
        public DateOnly? EventDateOf
        {
            get => Dateof;
        }
        public string EventDateOfString
        {
            get => string.Format("Дата проведения: {0}", Dateof);
        }
        public string EventDescription
        {
            get => string.Format("Описание проблемы: {0}", Description);
        }
        public string? EventStatus
        {
            get => string.Format("Статус: {0}",Eventstatus.Name);
        }
    }
    private List<EventPresenter> _events { get; set; }
    private ObservableCollection< EventPresenter> _displayEvents { get; set; }
    private string _searchWord;
    private EventPresenter _selectedItem;
    private int sortIndex;
    public EventWindow()
    {
        InitializeComponent();

        using (var dbcontext = new DiplomContext())
        {
            _events = dbcontext.Events.Include(e => e.Eventstatus)
                .Include(e => e.Eventstatus)
                .Include(e=>e.Equipment).Select(e =>
                new EventPresenter()
                {
                    Eventstatus = e.Eventstatus,
                    Dateof = e.Dateof,
                    Id = e.Id,
                    Equipment = e.Equipment,
                    
                    EquipmentId = e.EquipmentId,
                    Description = e.Description,
                    EventstatusId = e.EventstatusId
                }).ToList();
        }
        
        _displayEvents = new ObservableCollection<EventPresenter>(_events);
        EventListBox.ItemsSource = _displayEvents;

    }

    private void DipsplayEvents()
    {
        List<EventPresenter> _displayListEvents = new List<EventPresenter>(_events);

        if (!string.IsNullOrEmpty(_searchWord))
        {
            _displayListEvents = _displayListEvents.Where(e =>
                e.EventDescription.Contains(_searchWord) || e.Equipment.Name.Contains(_searchWord) ||
                e.Eventstatus.Name.Contains(_searchWord)).ToList(); 
        }

        if (sortIndex != null)
        {
            switch (sortIndex)
            {
                case 1:
                    _displayListEvents = _displayListEvents.Where(e => e.EventstatusId == 1).ToList();
                    break;
                case 2:
                    _displayListEvents = _displayListEvents.Where(e => e.EventstatusId == 2).ToList();
                    break;
                case 3:
                    _displayListEvents = _displayListEvents.Where(e => e.EventstatusId == 3).ToList();
                    break;
            }
        }
        
        if(_displayEvents.Count>0)
        {
            _displayEvents.Clear();
        }
        foreach (var events in _displayListEvents)
        {
            _displayEvents.Add(events);
        }
    }
    
    private void BackButtonClick(object? sender, RoutedEventArgs e)
    {
        new EquipmentWindow(1).Show();
        Hide();
    }

    private void EventListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _selectedItem = (EventPresenter)(sender as ListBox).SelectedItem;
        DeleteButton.IsVisible = true;
        EditButton.IsVisible = true;
    }

    private void SortComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        sortIndex = (sender as ComboBox).SelectedIndex;
        DipsplayEvents();
    }

    private void SearchTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        _searchWord = (sender as TextBox).Text;
        DipsplayEvents();
    }

    private async void EditButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var EditWin = new AddOrEditEventWindow(_selectedItem);
        var result = await EditWin.ShowDialog<EventPresenter>(this);
        if (result != null)
        {
            _selectedItem.Description = result.Description;
            _selectedItem.EquipmentId = result.EquipmentId;
            _selectedItem.EventstatusId = result.EventstatusId;
            _selectedItem.Dateof = result.Dateof;
            
            await using (var dbcontext = new DiplomContext())
            {
                var _event = dbcontext.Events.FirstOrDefault(e => e.Id == _selectedItem.Id);
                if (_event != null)
                {
                    _event.Description = result.Description;
                    _event.EquipmentId = result.EquipmentId;
                    _event.EventstatusId = result.EventstatusId;
                    _event.Dateof = result.Dateof;
                    _selectedItem.Eventstatus =
                        dbcontext.Eventstatuses.FirstOrDefault(es => es.Id == result.EventstatusId);
                    _selectedItem.Equipment = dbcontext.Equipments.FirstOrDefault(e => e.Id == result.EquipmentId);
                    switch (result.EventstatusId)
                    {
                        case 1:
                        {
                            var equipment = dbcontext.Equipments.FirstOrDefault(eq => eq.Id == result.EquipmentId);
                            if (equipment != null)
                            {
                                equipment.Dateoflastcheck = DateOnly.FromDateTime(DateTime.Now);
                                _events.Remove(_selectedItem);
                                dbcontext.Events.Remove(_event);
                                dbcontext.SaveChanges();
                            }

                            break;
                        }
                        case 3:
                            _events.Remove(_selectedItem);
                            dbcontext.Events.Remove(_event);
                            dbcontext.SaveChanges();
                            break;
                        case 2:
                            dbcontext.SaveChanges();
                            break;
                    }
                }
            }
        }
        DipsplayEvents();
            
    }

    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _events.Remove(_selectedItem);
        using (var dbcontext = new DiplomContext())
        {
            var _event = dbcontext.Events.FirstOrDefault(e => e.Id == _selectedItem.Id);
            dbcontext.Remove(_event);
            dbcontext.SaveChanges();
        }
        DipsplayEvents();
    }

    private async void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var addWin = new AddOrEditEventWindow();
        var result = await addWin.ShowDialog<EventPresenter>(this);
        if (result != null)
        {
            
            await using (var dbcontext = new DiplomContext())
            {
                var _event = new Event()
                {
                    Description = result.Description,
                    EventstatusId = result.EventstatusId,
                    Dateof = result.Dateof,
                    EquipmentId = result.EquipmentId
                };
                result.Equipment = dbcontext.Equipments
                    .First(ev => ev.Id == result.EquipmentId);
                result.Eventstatus = dbcontext.Eventstatuses
                    .First(s => s.Id == result.EventstatusId);
                if (result.EventstatusId == 1)
                {
                    var equipment = dbcontext.Equipments.FirstOrDefault(eq => eq.Id == result.EquipmentId);
                    if (equipment != null)
                    {
                        equipment.Dateoflastcheck = DateOnly.FromDateTime(DateTime.Now);
                        dbcontext.SaveChanges();
                    }
                }
                else if (result.EventstatusId == 3)
                {
                    return;
                }
                else
                {
                    dbcontext.Events.Add(_event);
                    dbcontext.SaveChanges();
                    _events.Add(result);
                }
            }
        }
        DipsplayEvents();
    }
}