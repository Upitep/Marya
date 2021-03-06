using Marya.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Marya.ViewModels
{
    public class DayViewModel : PropertyChangedAbs
    {
        public string MonthName => Days.LastOrDefault()?.Date?.ToString("MMMM");
        public ObservableCollection<DayVm> Days { get; set; }

        //В конструкторе реализована выгрузка данных из модели Day и преобразование их в формат данных, требуемых для работы с графикой
        public DayViewModel()
        {
            var month = new Month();
            Days = new ObservableCollection<DayVm>();

            if (month.Calendar.Count > 0)
            {
                var date = month.Calendar.First().Date;
                if (date.HasValue)
                {
                    var dayOfWeek = new DateTime(date.Value.Year, date.Value.Month, 1).DayOfWeek;
                    int margin = dayOfWeek - DayOfWeek.Monday;

                    if (margin > 0)
                    {
                        for (int i = 0; i < margin; i++)
                            Days.Add(new DayVm(new ObservableCollection<CityVm>()));
                    }

                    foreach (Day day in month.Calendar)
                    {
                        var cit = new ObservableCollection<CityVm>();
                        foreach (var cityy in day.CityList)
                        {
                            var slots = new ObservableCollection<FreeSlotVm>();
                            foreach (var slot in cityy.FreeSlotsList)
                            {
                                slots.Add(new FreeSlotVm(slot.Quantity, slot.StartInterval, slot.StopInterval));
                            }
                            
                            cit.Add(new CityVm(cityy.Name, slots));
                        }

                        Days.Add(new DayVm(cit, day.Date));
                    }
                }
            }
        }

        public class DayVm : PropertyChangedAbs
        {
            public DateTime? Date { get; set; }
            public string DateDayText => Date?.Day.ToString();
            public ObservableCollection<CityVm> TotalSlotsList { get; } = new ObservableCollection<CityVm>();

            private ObservableCollection<CityVm> _CityList;

            public ObservableCollection<CityVm> CityList
            {
                get => _CityList;
                set { _CityList = value; OnPropertyChanged(); }
            }
            private ObservableCollection<FreeSlotVm> _FreeSlotsList;
            public ObservableCollection<FreeSlotVm> FreeSlotsList
            {
                get => _FreeSlotsList;
                set
                {
                    _FreeSlotsList = value;
                    OnPropertyChanged();
                }
            }

            private int? _FreeSlots;
            public int? FreeSlots
            {
                get => _FreeSlots;
                set
                {
                    _FreeSlots = value;
                    OnPropertyChanged();
                }
            }

            public DayVm(ObservableCollection<CityVm> cityList, DateTime? date = null)
            {
                CityList = cityList;
                Date = date;

                foreach (var city in cityList)
                {
                    var slotvm = new ObservableCollection<FreeSlotVm>();
                    foreach (var slot in city.FreeSlotsList)
                    {
                        slotvm.Add(slot);
                    }

                    TotalSlotsList.Add(new CityVm(city.Name, slotvm));
                }
            }
        }

        public class CityVm : PropertyChangedAbs
        {
            public string Name { get; set; }
            private ObservableCollection<FreeSlotVm> _FreeSlotsList;

            public ObservableCollection<FreeSlotVm> FreeSlotsList
            {
                get => _FreeSlotsList;
                set { _FreeSlotsList = value; OnPropertyChanged(); }
            }
            public CityVm(string name, ObservableCollection<FreeSlotVm> freeSlotsList)
            {
                Name = name;
                FreeSlotsList = freeSlotsList;
            }

        }
        public class FreeSlotVm : PropertyChangedAbs
        {
            private int? _Quantity;
            public int? Quantity
            {
                get => _Quantity;
                set
                {
                    _Quantity = value;
                    OnPropertyChanged();
                }
            }
            public TimeSpan? StartInterval { get; set; }
            public TimeSpan? StopInterval { get; set; }
            public FreeSlotVm(int? quantity, TimeSpan? startInterval, TimeSpan? stopInterval)
            {
                Quantity = quantity;
                StartInterval = startInterval;
                StopInterval = stopInterval;
            }
        }
    }
}