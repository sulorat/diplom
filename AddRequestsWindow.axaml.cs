using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using diplom.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;

namespace diplom;

public partial class AddRequestsWindow : Window
{
    private RepairRequestWindow.RepairPresenter _repair = new RepairRequestWindow.RepairPresenter();
    public AddRequestsWindow()
    {
        InitializeComponent();

        using (var dbcontext = new DiplomContext())
        {
            var _equip = dbcontext.Equipments.Where(e=>e.Isdeleted!=true).ToList();
            var _priority = dbcontext.Priorities.ToList();
            var _statuses = dbcontext.Requeststatuses.ToList();

            StatusComboBox.ItemsSource = _statuses.Select(e => e.Name);
            PriorityComboBox.ItemsSource = _priority.Select(e => e.Name);
            EquipComboBox.ItemsSource = _equip.Select(e => e.Name);
        }
    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (EquipComboBox.SelectedItem == null || DescriptionTextBox.Text == null ||
            PriorityComboBox.SelectedItem == null || StatusComboBox.SelectedItem == null)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Заполните все поля");
            await messageBox.ShowAsync();
            return;
        }

        using (var dbcontext = new DiplomContext())
        {
            var equip = dbcontext.Equipments.FirstOrDefault(e => e.Name == EquipComboBox.SelectedItem);
            var status = dbcontext.Requeststatuses.FirstOrDefault(e => e.Name == StatusComboBox.SelectedItem);
            var priority = dbcontext.Priorities.FirstOrDefault(e => e.Name == PriorityComboBox.SelectedItem);

            if (equip != null || status != null || priority != null ||DescriptionTextBox.Text!=null)
            {
                Repairrequest rep = new Repairrequest();
                rep.Id = dbcontext.Repairrequests.Max(e => e.Id) + 1;
                rep.Description = DescriptionTextBox.Text;
                rep.Createdat = DateOnly.FromDateTime(DateTime.Now);
                rep.EquipmentId = equip.Id;
                rep.Equipment = equip;
                rep.Priority = priority.Id;
                rep.Status = status;
                rep.PriorityNavigation = priority;
                rep.StatusId = status.Id;
                dbcontext.Repairrequests.Add(rep);
                dbcontext.SaveChanges();
            }
            else
            {
                var messageBox = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Заполните все поля");
                await messageBox.ShowAsync();
                return;
            }
        }
        var win = new EquipmentWindow();
        win.Show();
        Hide();
        
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var win = new EquipmentWindow();
        win.Show();
        Hide();
    }
}