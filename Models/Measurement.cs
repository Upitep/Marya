using System;
using System.Collections.Generic;

namespace Marya.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string City { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime? Date { get; set; }
        public Measurement(int id, string orderNumber, string city, string customerName, string customerAddress, string customerNumber, DateTime? date)
        {
            Id = id;
            OrderNumber = orderNumber;
            City = city;
            CustomerName = customerName;
            CustomerAddress = customerAddress;
            CustomerNumber = customerNumber;
            Date = date;
        }
    }

    public class MeasurementsList
    {
        public List<Measurement> Measurements { get; set; }

        public MeasurementsList()
        {
            Measurements = new List<Measurement>
            {
                new Measurement(1, "1XXXXXX56", "Москва", "Иван", "ул. Ленина", "+79031234567", null),
                new Measurement(2, "2XXXXXX56", "Москва", "Альберт", "ул. Ленина", "+79081234567", null),
                new Measurement(3, "3XXXXXX56", "Москва", "Никита", "ул. Ленина", "+79111234567", null),
                new Measurement(4, "1XXXXXX66", "Саратов", "Роман", "ул. Ленина", "+79051234567", null),
                new Measurement(5, "2XXXXXX66", "Саратов", "Алексей", "ул. Ленина", "+79032235567", null),
                new Measurement(6, "3XXXXXX66", "Саратов", "Наташа", "ул. Ленина", "+79031224567", null),
            };
        }
    }
}
