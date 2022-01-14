using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Marya.ViewModels
{
    public class MainViewModel : PropertyChangedAbs, IDropTarget
    {
        public MainViewModel()
        {
            DayViewModel = new DayViewModel();
            MeasurementViewModel = new MeasurementViewModel(DayViewModel.Days);
            SelectedCity = CitiesList.FirstOrDefault();
        }

        private DayViewModel _DayViewModel;
        public DayViewModel DayViewModel
        {
            get => _DayViewModel;
            set
            { 
                _DayViewModel = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> CitiesList { get; } = new ObservableCollection<string>() { "Москва", "Саратов" };

        private string _SelectedCity;
        public string SelectedCity
        {
            get => _SelectedCity;
            set
            {
                _SelectedCity = value;
                CitySelection();

                OnPropertyChanged();
            }
        }
        private MeasurementViewModel _MeasurementViewModel;
        public MeasurementViewModel MeasurementViewModel
        {
            get => _MeasurementViewModel;
            set
            {
                _MeasurementViewModel = value;
                OnPropertyChanged();
            }
        }

        private DayViewModel.DayVm _SelectedDay;
        public DayViewModel.DayVm SelectedDay
        {
            get => _SelectedDay;
            set
            {
                _SelectedDay = value;
                SelectedDayInfo = new ObservableCollection<MeasurementViewModel.MeasurementVm>(MeasurementViewModel.Measurements.Where(x => (x?.Date?.Date == SelectedDay?.Date?.Date) && (SelectedDay?.Date?.Date != null) && (x?.City == SelectedCity)).ToList());
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MeasurementViewModel.MeasurementVm> _FreeMeasurements;
        public ObservableCollection<MeasurementViewModel.MeasurementVm> FreeMeasurements
        {
            get => _FreeMeasurements;
            set
            {
                _FreeMeasurements = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MeasurementViewModel.MeasurementVm> _SelectedDayInfo;
        public ObservableCollection<MeasurementViewModel.MeasurementVm> SelectedDayInfo
        {
            get => _SelectedDayInfo;
            set
            {
                _SelectedDayInfo = value;
                OnPropertyChanged();
            }
        }

        private void CitySelection()
        {
            SelectedDay = null;
            if (DayViewModel.Days?.Count > 0)
            {
                foreach (var day in DayViewModel.Days)
                {
                    var freeSlotsList = day.TotalSlotsList.FirstOrDefault(x => x.Name == SelectedCity)
                        ?.FreeSlotsList;
                    if (freeSlotsList != null)
                    {
                        day.FreeSlotsList = freeSlotsList;
                        day.FreeSlots = freeSlotsList.Any(x => x.Quantity != null)
                            ? freeSlotsList.Sum(x => x.Quantity)
                            : null;
                    }
                }
            }
            FreeMeasurements = new ObservableCollection<MeasurementViewModel.MeasurementVm>(MeasurementViewModel.Measurements.Where(x => x.Date == null && x.City == SelectedCity).ToList());
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.All;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data != null)
            {
                if (dropInfo.Data?.GetType() == typeof(MeasurementViewModel.MeasurementVm) && dropInfo.TargetItem?.GetType() == typeof(DayViewModel.DayVm)
                    && dropInfo.TargetItem != null && (DayViewModel.DayVm)dropInfo.TargetItem != null && ((DayViewModel.DayVm)dropInfo.TargetItem).Date != null
                    && ((DayViewModel.DayVm)dropInfo.TargetItem).FreeSlots > 0)
                {
                    if (SelectedDayInfo?.Contains(dropInfo.Data) == true && (DayViewModel.DayVm)dropInfo.TargetItem == SelectedDay) return;

                    var targetDay = (DayViewModel.DayVm)dropInfo.TargetItem;
                    var selectedMeasurement = (MeasurementViewModel.MeasurementVm)dropInfo.Data;

                    if (targetDay.FreeSlots == 0) return;
                    if (selectedMeasurement.Date != null && SelectedDay != null)
                    {
                        var selSlot = SelectedDay.FreeSlotsList.FirstOrDefault(x => x.StartInterval == selectedMeasurement.Interval.Time);
                        if (selSlot != null) selSlot.Quantity += 1;
                        SelectedDay.FreeSlots = SelectedDay.FreeSlotsList.Where(x => x.Quantity != null).Sum(x => x.Quantity);
                    }
                    selectedMeasurement.Date = targetDay.Date;
                    
                    SelectedDayInfo = new ObservableCollection<MeasurementViewModel.MeasurementVm>(MeasurementViewModel.Measurements.Where(x => (x?.Date?.Date == SelectedDay?.Date?.Date) && (SelectedDay?.Date?.Date != null) && (x?.City == SelectedCity)).ToList());
                    if (SelectedDayInfo.Count > 0)
                    {
                        foreach (var item in SelectedDayInfo)
                        {
                            item.PossibleIntervalStruct = MeasurementViewModel.GetSelectedMeasurementPossibleIntervalStruct(item);
                            item.Interval = MeasurementViewModel.GetSelectedMeasurementInterval(item);
                            if (item.Interval.Time == null) item.Interval.AlarmColor = (Brush)typeof(Brushes).GetProperty("Red")?.GetValue(null);
                        }
                    }
                    FreeMeasurements?.Remove(MeasurementViewModel.SelectedMeasurement);
                    SelectedDay = DayViewModel.Days.FirstOrDefault(x => x.Date?.Date == selectedMeasurement.Date?.Date);
                    MeasurementViewModel.SelectedMeasurement = selectedMeasurement;
                    OnPropertyChanged();
                }
                else if (dropInfo.Data.GetType() == typeof(MeasurementViewModel.MeasurementVm) && SelectedDay != null && dropInfo.TargetItem?.GetType() != typeof(DayViewModel.DayVm)
                    && FreeMeasurements.Contains(dropInfo.Data) != true)
                {
                    var selectedMeasurement = (MeasurementViewModel.MeasurementVm)dropInfo.Data;
                    var selSlot = SelectedDay.FreeSlotsList.FirstOrDefault(x => x.StartInterval == selectedMeasurement.Interval.Time);
                    if (selSlot != null) selSlot.Quantity += 1;
                    selectedMeasurement.Interval = null;
                    selectedMeasurement.Date = null;
                    SelectedDay.FreeSlots = SelectedDay.FreeSlotsList.Where(x => x.Quantity != null).Sum(x => x.Quantity);
                    FreeMeasurements.Add(MeasurementViewModel.SelectedMeasurement);
                    SelectedDayInfo.Remove(MeasurementViewModel.SelectedMeasurement);
                    OnPropertyChanged();
                }
            }
        }

        void IDropTarget.DragEnter(IDropInfo dropInfo)
        {
            
        }
        void IDropTarget.DragLeave(IDropInfo dropInfo)
        {

        }
    }
}
