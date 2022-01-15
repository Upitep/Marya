using System;
using Marya.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using GongSolutions.Wpf.DragDrop;
using Moq;


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

        [TestMethod]
        public void MakeDropTest()
        {
            var dropInfo = new Mock<IDropInfo>();
            var mainVm = new MainViewModel();
            mainVm.MeasurementViewModel.SelectedMeasurement = mainVm.FreeMeasurements.First();

            dropInfo.SetupGet(x => x.Data).Returns(mainVm.MeasurementViewModel.SelectedMeasurement);
            dropInfo.SetupGet(x => x.TargetItem).Returns(mainVm.DayViewModel.Days.Last());

            mainVm.MakeDrop(dropInfo.Object);
            Assert.AreEqual(mainVm.DayViewModel.Days.Last().Date, mainVm.MeasurementViewModel.SelectedMeasurement.Date);
            Assert.AreEqual(mainVm.DayViewModel.Days.Last().Date, mainVm.MeasurementViewModel.Measurements.First().Date);

            dropInfo.SetupGet(x => x.TargetItem).Returns(mainVm.DayViewModel.Days.First(x=>x.Date?.Date == new DateTime(2022,1,1)));
            mainVm.MakeDrop(dropInfo.Object);
            Assert.AreEqual(new DateTime(2022,1,1).Date, mainVm.MeasurementViewModel.SelectedMeasurement.Date);

            dropInfo.SetupGet(x => x.TargetItem).Returns(null);
            mainVm.MakeDrop(dropInfo.Object);
            Assert.AreEqual(null, mainVm.FreeMeasurements.First().Date);

        }
    }
}
