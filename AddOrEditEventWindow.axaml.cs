using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using diplom.Models;

namespace diplom;

public partial class AddOrEditEventWindow : Window
{
    private EventWindow.EventPresenter _eventPresenter = new EventWindow.EventPresenter();
    private string[] equipValues;
    private string?[] statuses;
    public AddOrEditEventWindow()
    {
        InitializeComponent();
        TitleTextBlock.Text = "Окно добавления";
        using (var dbcontext = new DiplomContext())
        {
            statuses = dbcontext.Eventstatuses.Select(e=>e.Description).ToArray();
            equipValues = dbcontext.Equipments.Select(e=>e.Name).ToArray();
        }
    }
    public AddOrEditEventWindow(EventWindow.EventPresenter _event)
    {
        InitializeComponent();

        using (var dbcontext = new DiplomContext())
        {
            statuses = dbcontext.Eventstatuses.Select(e=>e.Description).ToArray();
            equipValues = dbcontext.Equipments.Select(e=>e.Name).ToArray();
        }

        StatusComboBox.ItemsSource = statuses;
        EquipComboBox.ItemsSource = equipValues;
        _eventPresenter = _event;
        TitleTextBlock.Text = "Окно редактирования";
        DateCheckPicker.SelectedDate = new DateTimeOffset(_eventPresenter.Dateof.ToDateTime(TimeOnly.MinValue));
        
    }

    
    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _eventPresenter.Description = DescriptionTextBox.Text;
        _eventPresenter.EventstatusId = StatusComboBox.SelectedIndex + 1;
        _eventPresenter.EquipmentId = EquipComboBox.SelectedIndex + 1;
        _eventPresenter.Dateof = DateOnly.FromDateTime(DateCheckPicker.SelectedDate.Value.DateTime);
        Close(_eventPresenter);
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}