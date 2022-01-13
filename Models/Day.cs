using System;
using System.Collections.Generic;

namespace Marya.Models
{
    public class Day
    {
        public List<City> CityList { get; set; }
        public DateTime? Date { get; set; }
        public Day(List<City> cityList, DateTime? date = null)
        {
            CityList = cityList;
            Date = date;
        }
    }

    public class FreeSlot
    {
        public int? Quantity { get; set; }
        public TimeSpan? StartInterval { get; set; }
        public TimeSpan? StopInterval { get; set; }
        public FreeSlot(int? quantity, TimeSpan? startInterval, TimeSpan? stopInterval)
        {
            Quantity = quantity;
            StartInterval = startInterval;
            StopInterval = stopInterval;
        }
    }

    public class City
    {
        public string Name { get; set; }
        public List<FreeSlot> FreeSlotsList { get; set; }

        public City(string name, List<FreeSlot> freeSlotsList)
        {
            Name = name;
            FreeSlotsList = freeSlotsList;
        }
    }

    public class Month
    {
        public List<Day> Calendar { get; set; }

        public Month()
        {
            Calendar = new List<Day>();

            int daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            for (int i = 1; i <= daysInCurrentMonth; i++)
            {
                Calendar.Add(
                    new Day
                    (
                        new List<City>
                        {
                            new City("Москва",
                                new List<FreeSlot>
                                {
                                    new FreeSlot(3, new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)),
                                    new FreeSlot(2, new TimeSpan(12, 0, 0), new TimeSpan(14, 0, 0)),
                                    new FreeSlot(2, new TimeSpan(14, 0, 0), new TimeSpan(16, 0, 0)),
                                    new FreeSlot(3, new TimeSpan(16, 0, 0), new TimeSpan(18, 0, 0)),
                                }
                            ),

                            new City("Саратов",
                                new List<FreeSlot>
                                {
                                    new FreeSlot(2, new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)),
                                    new FreeSlot(1, new TimeSpan(12, 0, 0), new TimeSpan(14, 0, 0)),
                                    new FreeSlot(1, new TimeSpan(14, 0, 0), new TimeSpan(16, 0, 0)),
                                    new FreeSlot(2, new TimeSpan(16, 0, 0), new TimeSpan(18, 0, 0)),
                                }
                            ),
                        },
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, i)
                     )
                 );
            }
        }
    }
}
