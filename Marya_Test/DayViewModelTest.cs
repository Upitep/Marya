using Marya.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using System.Linq;


namespace Marya_Test
{
    [TestClass]
    public class DayViewModelTest
    {
        [TestMethod]
        public void DayViewModelConstructorTest()
        {
            DayViewModel month = new DayViewModel();

            Assert.AreEqual(true, month.MonthName != null);
            Assert.AreEqual(typeof(string), month.MonthName.GetType());
            Assert.AreEqual(true, month.Days != null);
            Assert.AreEqual(typeof(ObservableCollection<DayViewModel.DayVm>), month.Days.GetType());
            Assert.AreEqual(true, month.Days.Count > 0);
            Assert.AreEqual(month.MonthName, month.Days.Last().Date?.ToString("MMMM"));
        }

        [TestMethod]
        public void DayVmConstructorTest()
        {
            DayViewModel.FreeSlotVm freeSlot1 = new DayViewModel.FreeSlotVm(3, new TimeSpan(12, 0, 0), new TimeSpan(14, 0, 0));
            DayViewModel.FreeSlotVm freeSlot2 = new DayViewModel.FreeSlotVm(2, new TimeSpan(16, 0, 0), new TimeSpan(18, 0, 0));
            ObservableCollection<DayViewModel.FreeSlotVm> freeSlots = new ObservableCollection<DayViewModel.FreeSlotVm>{freeSlot1, freeSlot2};

            DayViewModel.CityVm city1 = new DayViewModel.CityVm("Москва", freeSlots);
            DayViewModel.CityVm city2 = new DayViewModel.CityVm("Саратов", freeSlots);

            ObservableCollection<DayViewModel.CityVm> cityList = new ObservableCollection<DayViewModel.CityVm>{city1, city2};

            DayViewModel.DayVm day = new DayViewModel.DayVm(cityList, new DateTime(2022, 01, 14));
            DayViewModel.DayVm day2 = new DayViewModel.DayVm(cityList);


            Assert.AreEqual(true, day.Date != null);
            Assert.AreEqual(new DateTime(2022,01,14),day.Date);
            Assert.AreEqual(true, day.CityList.Count > 0);
            Assert.AreEqual(cityList, day.CityList);

            Assert.AreEqual(true, day2.Date == null);
            Assert.AreEqual(cityList, day2.CityList);
        }
    }
}
