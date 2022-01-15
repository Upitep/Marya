using Marya.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace Marya.ViewModels
{
    public class MeasurementViewModel : PropertyChangedAbs
    {
        public ObservableCollection<DayViewModel.DayVm> Days { get; }
        public ObservableCollection<MeasurementVm> Measurements { get; }

        private MeasurementVm _SelectedMeasurement;
        public MeasurementVm SelectedMeasurement
        {
            get => _SelectedMeasurement;
            set
            {
                _SelectedMeasurement = value;
                if (_SelectedMeasurement != null)
                {
                    _SelectedMeasurement.PossibleIntervalStruct = GetSelectedMeasurementPossibleIntervalStruct(_SelectedMeasurement);
                    _SelectedMeasurement.Interval = GetSelectedMeasurementInterval(_SelectedMeasurement);
                }
                OnPropertyChanged();
            }
        }

        //Реализация полнотекстового поиска по всем доступным полям замера
        public ObservableCollection<MeasurementVm> FilteredMeasurements
        {
            get
            {
                if (SearchText == null) return Measurements;
                return new ObservableCollection<MeasurementVm>(Measurements
                    .Where(x => 
                        x.City.ToUpper().StartsWith(SearchText.ToUpper())
                        || x.OrderNumber.ToUpper().StartsWith(SearchText.ToUpper())
                        || x.CustomerName.ToUpper().StartsWith(SearchText.ToUpper())
                        || x.CustomerAddress.ToUpper().StartsWith(SearchText.ToUpper())
                        || x.CustomerNumber.ToUpper().StartsWith(SearchText.ToUpper())
                        || (DateTime.TryParse(SearchText, out var tempDate) && tempDate == x.Date)
                        || (x.Interval?.String != null && x.Interval.String.StartsWith(SearchText))
                        ).ToList());
            }
        }

        private string _SearchText = "";
        public string SearchText
        {
            get => _SearchText;
            set
            {
                _SearchText = value;

                OnPropertyChanged();
                OnPropertyChanged("FilteredMeasurements");
            }
        }

        //В конструкторе реализована выгрузка данных из модели Measurement и преобразование их в формат, требующийся для работы с граифкой
        public MeasurementViewModel(ObservableCollection<DayViewModel.DayVm> days)
        {
            Days = days;
            var measurementsList = new MeasurementsList();
            Measurements = new ObservableCollection<MeasurementVm>();

            if (measurementsList.Measurements.Count > 0)
            {
                foreach (var measurement in measurementsList.Measurements)
                {
                    Measurements.Add(new MeasurementVm(Days, measurement.Id, measurement.OrderNumber, measurement.City, measurement.CustomerName, measurement.CustomerAddress, measurement.CustomerNumber, measurement.Date));
                }
            }
        }

        public class MeasurementVm : PropertyChangedAbs
        {
            public ObservableCollection<DayViewModel.DayVm> Days { get; set; }

            public int Id { get; set; }
            private string _OrderNumber;
            public string OrderNumber
            {
                get => _OrderNumber;
                set { _OrderNumber = value; OnPropertyChanged(); }
            }
            private string _City;
            public string City
            {
                get => _City;
                set { _City = value; OnPropertyChanged(); }
            }
            private string _CustomerName;
            public string CustomerName
            {
                get => _CustomerName;
                set { _CustomerName = value; OnPropertyChanged(); }
            }
            private string _CustomerAddress;
            public string CustomerAddress
            {
                get => _CustomerAddress;
                set { _CustomerAddress = value; OnPropertyChanged(); }
            }
            private string _CustomerNumber;
            public string CustomerNumber
            {
                get => _CustomerNumber;
                set { _CustomerNumber = value; OnPropertyChanged(); }
            }
            private ObservableCollection<IntervalStruct> _PossibleIntervalStruct;
            public ObservableCollection<IntervalStruct> PossibleIntervalStruct
            {
                get => _PossibleIntervalStruct;
                set { _PossibleIntervalStruct = value; OnPropertyChanged(); }
            }

            private IntervalStruct _Interval = new IntervalStruct();
            public IntervalStruct Interval
            {
                get => _Interval;
                set
                {
                    IntervalSelection(value);
                    _Interval = value;
                    OnPropertyChanged();
                }
            }

            private DateTime? _Date;
            public DateTime? Date
            {
                get => _Date;
                set
                {
                    _Date = value;
                    OnPropertyChanged();
                }
            }

            public MeasurementVm(ObservableCollection<DayViewModel.DayVm> days, int id, string orderNumber, string city, string customerName, string customerAddress, string customerNumber, DateTime? date)
            {
                Days = days;
                Id = id;
                OrderNumber = orderNumber;
                City = city;
                CustomerName = customerName;
                CustomerAddress = customerAddress;
                CustomerNumber = customerNumber;
                Date = date;
            }

            //Реализация присовения интервала конкретному измерению, а так же пересчет доступных слотов измерений в указанном дне, а также реализация смены интервала в пределах выделенного дня
            public void IntervalSelection(IntervalStruct value)
            {
                if (value != null && Date != null)
                {
                    if (Interval.Time != null && Interval.Time != value.Time && value.Time != null)
                    {
                        var day = Days.FirstOrDefault(x => x.Date?.Date == Date.Value.Date);
                        if (day != null && Interval.Time != value.Time)
                        {
                            Date = Date.Value.Date + value.Time;

                            var quant = day.FreeSlotsList.FirstOrDefault(x => x.StartInterval == Interval.Time);
                            if (quant != null) quant.Quantity += 1;
                            quant = day.FreeSlotsList.FirstOrDefault(x => x.StartInterval == value.Time);
                            if (quant != null) quant.Quantity -= 1;
                            day.FreeSlots = day.FreeSlotsList.Where(x => x.Quantity != null).Sum(x => x.Quantity);
                        }
                    }
                    else if (value.Time != null)
                    {
                        Date = Date.Value.Date + value.Time;
                        var day = Days.FirstOrDefault(x => x.Date?.Date == Date.Value.Date);
                        if (day != null && Interval.Time != value.Time)
                        {
                            var quant = day.FreeSlotsList.FirstOrDefault(x => x.StartInterval == value.Time);
                            if (quant != null) quant.Quantity -= 1;
                            day.FreeSlots = day.FreeSlotsList.Where(x => x.Quantity != null).Sum(x => x.Quantity);
                        }
                    }
                }
            }
        }

        //Класс интервала, а также перегрузка методов сравнения, т.к. требуется для работы графики и прохождения тестов
        public sealed class IntervalStruct
        {
            public string String { get; set; }
            public TimeSpan? Time { get; set; }
            public Brush AlarmColor { get; set; }

            public IntervalStruct(string str = null, TimeSpan? time = null, Brush alarmColor = null)
            {
                String = str;
                Time = time;
                AlarmColor = alarmColor ?? (Brush)typeof(Brushes).GetProperty("Red")?.GetValue(null);
            }

            private static int GetHashCodeHelper(int[] subCodes)
            {
                if ((object)subCodes == null || subCodes.Length == 0)
                    return 0;

                int result = subCodes[0];

                for (int i = 1; i < subCodes.Length; i++)
                    result = unchecked(result * 397) ^ subCodes[i];

                return result;
            }
            public override int GetHashCode() => GetHashCodeHelper(
                new int[]
                {
                    String == null ? 0 : String.GetHashCode(),
                    Time.GetHashCode(),
                    AlarmColor.GetHashCode(),
                }
            );

            private static bool EqualsHelper(IntervalStruct first, IntervalStruct second) =>
                first.String == second.String &&
                first.Time == second.Time &&
                first.AlarmColor == second.AlarmColor;

            public bool Equals(IntervalStruct other)
            {
                if ((object)this == (object)other)
                    return true;

                if ((object)other == null)
                    return false;

                if (this.GetType() != other.GetType())
                    return false;

                return EqualsHelper(this, other);
            }

            public override bool Equals(object obj) => this.Equals(obj as IntervalStruct);

            public static bool Equals(IntervalStruct first, IntervalStruct second) =>
                first?.Equals(second) ?? (object)first == (object)second;

            public static bool operator ==(IntervalStruct first, IntervalStruct second) => Equals(first, second);

            public static bool operator !=(IntervalStruct first, IntervalStruct second) => !Equals(first, second);

        }

        //Метод, реализацющий получение текущего интервала исходя из полной даты измерения, передаваемого в качестве аргумента. Требуется для заполнение комбобокса интервала
        public IntervalStruct GetSelectedMeasurementInterval(MeasurementVm selectedMeasurement)
        {
            var result = new IntervalStruct();
            if (selectedMeasurement != null)
            {
                var date = selectedMeasurement.Date?.Date;
                var time = selectedMeasurement.Date?.TimeOfDay;
                var city = selectedMeasurement.City;

                if (time != null && Days != null && date != null)
                {
                    var day = Days.FirstOrDefault(x => x.Date?.Date == date);

                    if (day != null && day.TotalSlotsList.Count > 0)
                    {
                        var freeSlotsList = day.TotalSlotsList.FirstOrDefault(x => x.Name == city)
                            ?.FreeSlotsList;
                        if (freeSlotsList != null)
                        {
                            foreach (var daySlot in freeSlotsList)
                            {
                                if (daySlot.StartInterval <= time && time < daySlot.StopInterval)
                                {
                                    result.String = $"{daySlot.StartInterval?.Hours} - {daySlot.StopInterval?.Hours}";
                                    result.Time = daySlot.StartInterval;
                                    result.AlarmColor =
                                        (Brush)typeof(Brushes).GetProperty("Transparent")?.GetValue(null);
                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        //Метод, реализующий получение списка доступных интервалов замеров по указанному измерению и доступным слотам выбранного дня. Требуется для заполнения списка комбобокса интервалов
        public ObservableCollection<IntervalStruct> GetSelectedMeasurementPossibleIntervalStruct(MeasurementVm selectedMeasurement)
        {
            var resultList = new ObservableCollection<IntervalStruct>();

            if (selectedMeasurement != null)
            {
                var date = selectedMeasurement.Date?.Date;
                var day = Days.FirstOrDefault(x => x.Date?.Date == date);

                if (day != null && day.FreeSlotsList?.Count > 0)
                {
                    foreach (var daySlot in day.FreeSlotsList)
                    {
                        if (daySlot.Quantity > 0 && daySlot.StartInterval.HasValue && daySlot.StopInterval.HasValue)
                        {
                            resultList.Add(new IntervalStruct(
                                $"{daySlot.StartInterval.Value.Hours} - {daySlot.StopInterval.Value.Hours}",
                                daySlot.StartInterval.Value,
                                (Brush)typeof(Brushes).GetProperty("Transparent")?.GetValue(null)));
                        }
                    }

                    if (selectedMeasurement.Interval?.Time != null) resultList.Add(selectedMeasurement.Interval);

                    resultList =
                        new ObservableCollection<IntervalStruct>(resultList.Union(resultList).ToList()
                            .OrderBy(x => x.Time));
                }
            }
            return resultList;
        }

    }
}
