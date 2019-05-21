using System;
using System.Collections.Generic;
using System.IO;

namespace RootData.Models
{
    public class ReportData
    {
        public string DriverName { get; private set; }
        public int TotalDistance { get; private set; }
        public int Speed { get; private set; }

        public ReportData(string driverName, int totalDistance, int averageSpeed)
        {
            this.DriverName = driverName;
            this.TotalDistance = totalDistance;
            this.Speed = averageSpeed;
        }        
        public static void WriteReport(List<ReportData> reportData, string outputPath, string outputFile)
        {
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            using (var sw = new StreamWriter(Path.Combine(outputPath, outputFile), false))
            {
                foreach (var rd in reportData) {
                    if (rd.TotalDistance > 0)
                        sw.WriteLine($"{rd.DriverName}: " + $"{rd.TotalDistance} " + "miles " + "@ " + $"{rd.Speed} " + $"mph");
                    else
                         sw.WriteLine($"{rd.DriverName}: " + $"{rd.TotalDistance} " + "miles");
                }
            }
        }
        public static List<ReportData> PopulateReportData(List<Driver> drivers)
        {
            var result = new List<ReportData>();

            foreach (var d in drivers) {
                decimal totalDistance = 0M;
                double totalHours = 0;
                     
                if (d.Trips.Count > 0)
                {
                    foreach (var t in d.Trips) {
                        totalDistance += t.Distance;
                        totalHours += (t.EndTime - t.StartTime).TotalHours;
                    }                 
                    //calculate avgSpeed before rounding totalDistance                   
                    var avgSpeed = CalculateAverageSpeed(totalDistance, totalHours); 
                    var roundedDistance = RoundDistance(totalDistance);
                    result.Add(new ReportData(d.Name, roundedDistance, avgSpeed));               
                }
                else
                    result.Add(new ReportData(d.Name, 0, 0));
            }
            return result;
        }

        public static void SortReportDataByMileage(List<ReportData> data)
            => data.Sort((x, y) => y.TotalDistance.CompareTo(x.TotalDistance));

        public static int CalculateAverageSpeed(decimal totalDistance, double totalHours)  
            => (int)Math.Round((totalDistance / (decimal)totalHours), MidpointRounding.AwayFromZero);
        
        public static int RoundDistance(decimal distance) 
            => (int)Math.Round(distance, MidpointRounding.AwayFromZero);

    }   
}
