using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using diplom.Models;

namespace diplom;

public partial class HistoryMovementWindow : Window
{
    public class MovementPresenter : Equipmentmovement
    {
        public string MoveEquip
        {
            get => string.Format("Название: {0}",Equipment.Name);
        }
        public string MoveType
        {
            get => string.Format("Тип перемещения: {0}",Type.Name);
        }
        public string MoveFromDateString
        {
            get => string.Format("Перемещение от: {0}",Fromdate);
        }
        public DateOnly? MoveFromDate
        {
            get => Fromdate;
        }
        public string MoveToDateString
        {
            get => string.Format("Перемещение до: {0}",Todate);
        }
        public DateOnly? MoveToDate
        {
            get => Todate;
        }
        public string  MovePerson
        {
            get => string.Format("Ответсвтенный сотрудник: {0}", ResponsiblepersonNavigation.Name);
        }
        public string MoveTo
        {
            get => string.Format("Бывшее местоположение: {0}", Fromplace);
        }
        public string MoveFrom
        {
            get => string.Format("Настоящее местоположение: {0}", Toplace);
        }
        public string MoveReason
        {
            get => string.Format("Причина: {0}", Reason);
        }
    }
    private List<MovementPresenter> _movement { get; set; }
    private ObservableCollection<MovementPresenter> _displayMovement { get; set; }
    private string[] sortValues = ["Все", "Перемещено", "Списано", "Выдано на руки сотруднику"];
    private int sortIndex;
    public HistoryMovementWindow()
    {
        InitializeComponent();

        using (var dbcontext = new DiplomContext())
        {
            _movement = dbcontext.Equipmentmovements.Select(e => new MovementPresenter()
            {
                Equipment = e.Equipment,
                EquipmentId = e.EquipmentId,
                ResponsiblepersonNavigation = e.ResponsiblepersonNavigation,
                Reason = e.Reason,
                Fromdate = e.Fromdate,
                Responsibleperson = e.Responsibleperson,
                Todate = e.Todate,
                Toplace = e.Toplace,
                Fromplace = e.Fromplace,
                Type = e.Type,
                TypeId = e.TypeId
            }).ToList();
        }

        SortComboBox.ItemsSource = sortValues;
        _displayMovement = new ObservableCollection<MovementPresenter>(_movement);
        HistoryListBox.ItemsSource = _displayMovement;
    }

    private void DisplayHistory()
    {
        List<MovementPresenter> displayList = new List<MovementPresenter>(_movement);

        switch (sortIndex)
        {
            case 1:
                displayList = displayList.Where(e => e.TypeId == 1).ToList();
                break;
            case 2:
                displayList = displayList.Where(e => e.TypeId == 2).ToList();
                break;
            case 3:
                displayList = displayList.Where(e => e.TypeId == 3).ToList();
                break;
        }
        
        if (_displayMovement.Count > 0)
        {
            _displayMovement.Clear();
        }
        foreach (var moves in displayList)
        {
            _displayMovement.Add(moves);
        }
    }
    
    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var win = new MovementWindow();
        win.Show();
        Hide();
    }

    private void SortComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        sortIndex = (sender as ComboBox).SelectedIndex;
        DisplayHistory();
    }
}