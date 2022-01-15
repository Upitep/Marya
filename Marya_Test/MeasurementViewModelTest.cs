using Marya.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;


namespace Marya_Test
{
    [TestClass]
    public class MeasurementViewModelTest
    {
        [TestMethod]
        public void MeasurementViewModelConstructorTest()
        {
            var days = new DayViewModel().Days;
            var measurements = new MeasurementViewModel(days);

            Assert.AreEqual(true, measurements.Days != null);
            Assert.AreEqual(typeof(ObservableCollection<DayViewModel.DayVm>), measurements.Days.GetType());
            Assert.AreEqual(true, measurements.Measurements != null);
            Assert.AreEqual(typeof(ObservableCollection<MeasurementViewModel.MeasurementVm>), measurements.Measurements.GetType());

            Assert.AreEqual(true, measurements.Days.Count > 0);
            Assert.AreEqual(true, measurements.Measurements.Count > 0);
        }

        [TestMethod]
        public void GetSelectedMeasurementIntervalTest()
        {
            var dayvm = new DayViewModel();
            var measurements = new MeasurementViewModel(dayvm.Days);

            measurements.Measurements.Last().Date = new DateTime(2022, 1, 31,12,0,0);
            var testInterval1 = new MeasurementViewModel.IntervalStruct("12 - 14", new TimeSpan(12, 0, 0), (Brush)typeof(Brushes).GetProperty("Transparent")?.GetValue(null));
            var testInterval2 = new MeasurementViewModel.IntervalStruct();

            var newInterval = measurements.GetSelectedMeasurementInterval(measurements.Measurements.Last());
            Assert.AreEqual(testInterval1, newInterval);

            var nullMeasurement = measurements.Measurements.First();
            newInterval = measurements.GetSelectedMeasurementInterval(nullMeasurement);
            Assert.AreEqual(testInterval2, newInterval);

            measurements.Measurements.Last().Date = new DateTime(2023, 1, 31, 12, 0, 0);
            newInterval = measurements.GetSelectedMeasurementInterval(measurements.Measurements.Last());
            Assert.AreEqual(testInterval2, newInterval);

            measurements.Measurements.Last().Date = new DateTime(2022, 1, 31, 12, 0, 0);
            measurements.Measurements.Last().City = "Рязань";
            newInterval = measurements.GetSelectedMeasurementInterval(measurements.Measurements.Last());
            Assert.AreEqual(testInterval2, newInterval);

            measurements.Measurements.First().Date = new DateTime(2022, 1, 14, 22, 0, 0);
            newInterval = measurements.GetSelectedMeasurementInterval(measurements.Measurements.First());
            Assert.AreEqual(testInterval2, newInterval);
        }

        [TestMethod]
        public void GetSelectedMeasurementPossibleIntervalStructTest()
        {
            var dayvm = new DayViewModel();
            var measurements = new MeasurementViewModel(dayvm.Days);

            var test1IntervalList = new ObservableCollection<MeasurementViewModel.IntervalStruct>
            {
                new MeasurementViewModel.IntervalStruct("10 - 12", new TimeSpan(10, 0, 0), (Brush)typeof(Brushes).GetProperty("Transparent")?.GetValue(null)),
                new MeasurementViewModel.IntervalStruct("12 - 14", new TimeSpan(12, 0, 0), (Brush)typeof(Brushes).GetProperty("Transparent")?.GetValue(null)),
                new MeasurementViewModel.IntervalStruct("14 - 16", new TimeSpan(14, 0, 0), (Brush)typeof(Brushes).GetProperty("Transparent")?.GetValue(null)),
                new MeasurementViewModel.IntervalStruct("16 - 18", new TimeSpan(16, 0, 0), (Brush)typeof(Brushes).GetProperty("Transparent")?.GetValue(null)),
            };

            measurements.Measurements.First().Date = new DateTime(2022, 1, 31, 12, 0, 0);
            measurements.Days.Last().FreeSlotsList = new ObservableCollection<DayViewModel.FreeSlotVm>
            {
                new DayViewModel.FreeSlotVm(2, new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)),
                new DayViewModel.FreeSlotVm(1, new TimeSpan(12, 0, 0), new TimeSpan(14, 0, 0)),
                new DayViewModel.FreeSlotVm(1, new TimeSpan(14, 0, 0), new TimeSpan(16, 0, 0)),
                new DayViewModel.FreeSlotVm(2, new TimeSpan(16, 0, 0), new TimeSpan(18, 0, 0)),
            };

            var newIntervalList = measurements.GetSelectedMeasurementPossibleIntervalStruct(measurements.Measurements.First());
            Assert.IsTrue(test1IntervalList.SequenceEqual(newIntervalList));

            measurements.Measurements.Last().Date = new DateTime(2023, 1, 31, 12, 0, 0);
            var test2IntervalList = new ObservableCollection<MeasurementViewModel.IntervalStruct>();
            newIntervalList = measurements.GetSelectedMeasurementPossibleIntervalStruct(measurements.Measurements.Last());
            Assert.IsTrue(test2IntervalList.SequenceEqual(newIntervalList));

            measurements.Days.Last().FreeSlotsList = new ObservableCollection<DayViewModel.FreeSlotVm>
            {
                new DayViewModel.FreeSlotVm(null, null, null),
            };
            newIntervalList = measurements.GetSelectedMeasurementPossibleIntervalStruct(measurements.Measurements.First());
            Assert.IsTrue(test2IntervalList.SequenceEqual(newIntervalList));
        }

        [TestMethod]
        public void IntervalSelectionTest()
        {
            var dayvm = new DayViewModel();
            var measurements = new MeasurementViewModel(dayvm.Days);

            measurements.Measurements.First().Date = new DateTime(2022, 1, 31);
            measurements.Measurements.First().Interval = new MeasurementViewModel.IntervalStruct();
            measurements.Days.Last().FreeSlotsList = new ObservableCollection<DayViewModel.FreeSlotVm>
            {
                new DayViewModel.FreeSlotVm(2, new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)),
                new DayViewModel.FreeSlotVm(1, new TimeSpan(12, 0, 0), new TimeSpan(14, 0, 0)),
                new DayViewModel.FreeSlotVm(1, new TimeSpan(14, 0, 0), new TimeSpan(16, 0, 0)),
                new DayViewModel.FreeSlotVm(2, new TimeSpan(16, 0, 0), new TimeSpan(18, 0, 0)),
            };
            var testInterval = new MeasurementViewModel.IntervalStruct("12 - 14", new TimeSpan(12, 0, 0), (Brush)typeof(Brushes).GetProperty("Transparent")?.GetValue(null));

            measurements.Measurements.First().IntervalSelection(testInterval);
            Assert.AreEqual(5, dayvm.Days.Last().FreeSlots);
            Assert.AreEqual(new DateTime(2022,1,31,12,0,0), measurements.Measurements.First().Date);

            measurements.Measurements.First().Interval.String = testInterval.String;
            measurements.Measurements.First().Interval.Time = testInterval.Time;
            measurements.Measurements.First().Interval.AlarmColor = testInterval.AlarmColor;
            testInterval = new MeasurementViewModel.IntervalStruct("16 - 18", new TimeSpan(16, 0, 0), (Brush)typeof(Brushes).GetProperty("Transparent")?.GetValue(null));
            measurements.Measurements.First().IntervalSelection(testInterval);
            Assert.AreEqual(5, dayvm.Days.Last().FreeSlots);
            Assert.AreEqual(new DateTime(2022, 1, 31, 16, 0, 0), measurements.Measurements.First().Date);

            measurements.Measurements.First().IntervalSelection(testInterval);
            Assert.AreEqual(5, dayvm.Days.Last().FreeSlots);
            Assert.AreEqual(new DateTime(2022, 1, 31, 16, 0, 0), measurements.Measurements.First().Date);

        }
    }
}
