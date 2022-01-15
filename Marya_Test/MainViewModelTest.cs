using Marya.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;


namespace Marya_Test
{
    [TestClass]
    public class MainViewModelTest
    {
        [TestMethod]
        public void CitySelectionTest()
        {
            var mainVm = new MainViewModel();

            Assert.AreEqual("Москва", mainVm.SelectedCity);
            Assert.AreEqual(10, mainVm.DayViewModel.Days.Last().FreeSlots);
            Assert.AreEqual(3, mainVm.FreeMeasurements.Count);
            Assert.AreEqual(null, mainVm.SelectedDay);

            mainVm.SelectedCity = mainVm.CitiesList.Last();
            Assert.AreEqual("Саратов", mainVm.SelectedCity);
            Assert.AreEqual(6, mainVm.DayViewModel.Days.Last().FreeSlots);
            Assert.AreEqual(3, mainVm.FreeMeasurements.Count);
        }
    }
}
