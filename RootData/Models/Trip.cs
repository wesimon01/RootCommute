using System;
using System.Collections.Generic;
using System.Globalization;

namespace RootData.Models
{
    public class Trip : RootCommand
    {
        public string DriverName { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public decimal Distance { get; private set; }

        public Trip(string[] data)
        {
            if (!(this.IsValid = IsValidData(data)))
                return;
            
            this.DriverName = data[1];
            this.StartTime = DateTime.ParseExact(data[2], "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;
            this.EndTime = DateTime.ParseExact(data[3], "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;
            this.Distance = decimal.Parse(data[4]);
        }
        protected override bool IsValidData(string[] data)
        {
            if (data.Length != 5)
                return false;
            
            if (string.IsNullOrWhiteSpace(data[1]))
                return false;
            if (!DateTime.TryParseExact(data[2],"HH:mm", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startTime))
                return false;             
            if (!DateTime.TryParseExact(data[3],"HH:mm", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endTime))
                return false;                           
            if (!decimal.TryParse(data[4], out decimal distance))
                return false;
            
            if (startTime.TimeOfDay >= endTime.TimeOfDay)
                return false;

            return true;
        }
        public static List<Trip> FilterTripsBySpeed(List<Trip> trips)
        {
            var result = new List<Trip>();
            foreach(var t in trips)
            {
                var speed = CalculateSpeedMph(t);
                if (speed >= 5M && speed <= 100M)
                    result.Add(t);
            }
            return result;
        }
        public static decimal CalculateSpeedMph(Trip trip)
        {             
           var hours = (trip.EndTime - trip.StartTime).TotalHours;            
           return (trip.Distance / (decimal)hours);
        }
    }
}
