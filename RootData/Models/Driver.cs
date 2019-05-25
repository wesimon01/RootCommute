using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RootData.Models
{
    public class Driver : RootCommand
    {
        public string Name { get; private set; }
        public List<Trip> Trips { get; private set; }
              
        public Driver(string[] data)
        {
            if (!(this.IsValid = IsValidData(data)))
                return;

            this.Name = data[1];
            this.Trips = new List<Trip>();
        }      
        protected override bool IsValidData(string[] data)
        {
            if (data.Length != 2)
                return false;            
            if (string.IsNullOrWhiteSpace(data[1]))
                return false;

            return true;
        }       
        public static void CheckForDuplicateDrivers(List<Driver> drivers)
        {
            var dups = drivers.GroupBy(d => d.Name)
                       .Where(d => d.Count() > 1)
                       .Select(d => new { driverName = d.Key + " "})
                       .ToList();

            if (dups.Count() > 0)
                throw new InvalidDataException($"Duplicate driver names found in input data: {dups.ToString()}");
        }
        public static void AssignTripsToDrivers(List<Driver> drivers, List<Trip> trips)
        {      
            foreach (var d in drivers) {
                var driverTrips = trips.Where(t => t.DriverName == d.Name);
                d.Trips.AddRange(driverTrips);
            }

            //foreach (var d in drivers) {
            //    foreach (var t in trips) {
            //        if (d.Name == t.DriverName)                    
            //            d.Trips.Add(t);                                          
            //    }
            //}
        }
    }
}
