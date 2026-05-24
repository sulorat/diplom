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
using Microsoft.EntityFrameworkCore;

namespace diplom;

public partial class HistoryEquipWindow : Window
{
    public class HistoryEquipPresenter :Equipment
    {
        public string? EquipImagePath
        {
            get => Imagepath;
        }

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

    
    private List<HistoryEquipPresenter> _equipments { get; set; }
    
    public HistoryEquipWindow()
    {
        InitializeComponent();

        using (var dbcotext = new DiplomContext())
        {
            _equipments = dbcotext.Equipments.Select((e =>
                new HistoryEquipPresenter()
                {
                    Name = e.Name,
                    Imagepath = e.Imagepath,
                    Status = e.Status,
                    StatusId = e.StatusId,
                    Place = e.Place,    
                    Workhours = e.Workhours,
                    Equipmentmovements = e.Equipmentmovements,
                    Productioncapacity = e.Productioncapacity,
                    Isdeleted = e.Isdeleted,
                    Dateoflastcheck = e.Dateoflastcheck,
                    Id = e.Id
                })).ToList();
            
            HistoryListBox.ItemsSource = _equipments;
        }
    }

    
    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        new EquipmentWindow(1).Show();
        Hide();
    }
}