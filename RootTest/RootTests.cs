using NUnit.Framework;
using RootData.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    public class RootTests 
    {
        private const string delimiter = " "; 

        [Test]
        public void CanParseCommand()
        {
            //Arrange
            var cmdStr1 = "Driver Evan"; 
            var cmdStr2 = "Trip James 12:00 14:00 55.8";          
            //Act            
            Driver parsedCmd1 = (Driver)RootCommand.ParseCommand(cmdStr1, delimiter);
            Trip parsedCmd2 = (Trip)RootCommand.ParseCommand(cmdStr2, delimiter);
            
            //Assert
            Assert.AreEqual("Evan", parsedCmd1.Name);
            Assert.AreEqual("James", parsedCmd2.DriverName);
            Assert.AreEqual(new TimeSpan(12, 0, 0), parsedCmd2.StartTime);
            Assert.AreEqual(new TimeSpan(14, 0, 0), parsedCmd2.EndTime);
            Assert.AreEqual(55.8M, parsedCmd2.Distance);
            
        }
        [Test]
        public void CanFilterTripsBySpeed()
        {
            //Arrange
            var unfilteredTripCmds = new List<Trip>()
            {
                new Trip(new string[]{"Trip", "Ted", "05:00", "05:30", "2.1"}),
                new Trip(new string[]{"Trip", "Ted", "05:00", "08:00", "200.8"}),
                new Trip(new string[]{"Trip", "Ted", "05:00", "08:00", "200.8"}), 
                new Trip(new string[]{"Trip", "Ted", "05:00", "06:00", "4.5"})
            };

            //Act
            var trips = Trip.FilterTripsBySpeed(unfilteredTripCmds); 

            //Assert
            Assert.IsTrue(trips.Count == 2);
        }
        [Test]
        public void CanCalculateAverageSpeed()
        {
            //Arrange
            var totalDistance = 500M;
            var totalHours = new TimeSpan(5, 30, 0).TotalHours;
            //Act
            int speed = ReportData.CalculateAverageSpeed(totalDistance, totalHours);

            //Assert
            Assert.AreEqual(91, speed);
        }
        [Test]
        public void TestIsValidDriverData()
        {
            //Arrange
            //Act
            var driver1 = new Driver(new string[] { "DriverSpaceEvan" });
            var driver2 = new Driver(new string[] { "Driver", "   " });
            var driver3 = new Driver(new string[] { "Driver", "Guy" });
            
            //Assert
            Assert.False(driver1.IsValid);
            Assert.False(driver2.IsValid);
            Assert.IsTrue(driver3.IsValid);
        }
        [Test]
        public void TestIsValidTripData()
        {
            //Arrange
            //Act
            var trip1 = new Trip(new string[] { "Trip", "Ted", "05:00", "08:00" });
            var trip2 = new Trip(new string[] { "Trip", "Ted", "03:00", "02:00", "200.8" });
            var trip3 = new Trip(new string[] { "Trip", " ", "05:00", "08:00", "200.8" });
            var trip4 = new Trip(new string[] { "Trip", " ", "55:00", "08:00", "200.8" });
            var trip5 = new Trip(new string[] { "Trip", "Ted", "05:00", "08:00", "200.8"});
            
            //Assert
            Assert.False(trip1.IsValid);
            Assert.False(trip2.IsValid);    
            Assert.False(trip3.IsValid);
            Assert.False(trip4.IsValid);
            Assert.IsTrue(trip5.IsValid);
        }
        [Test] 
        public void CanGroupCommandsByType() 
        { 
            //Arrange
            var input = new string[][]
            {
               new string [] { "Driver", "Pepe" },
               new string [] { "Driver", "Ted" },
               new string [] { "Trip", "Pepe", "12:00", "14:00", "55.8" },
               new string [] { "Trip", "Ted", "05:00", "08:00", "200.8" },
               new string [] { "Trip", "Ted", "12:00", "14:00", "109.8" }
            };
            var commands = new List<RootCommand>();
            var drivers = new List<Driver>();
            var trips = new List<Trip>();

            commands.Add(new Driver(input[0]));
            commands.Add(new Driver(input[1]));
            commands.Add(new Trip(input[2]));
            commands.Add(new Trip(input[3]));
            commands.Add(new Trip(input[4]));
            
            //Act
            RootCommand.GroupCommandsByType(commands, drivers, trips);

            //Assert
            Assert.IsTrue(drivers.Count == 2);
            Assert.IsTrue(trips.Count == 3);
        }
        [Test]
        public void CanDetectDuplicateDrivers()
        {
            //Arrange
            var driverCmds = new Driver[]
            {
                new Driver(new string[] {"Driver", "Ted" }),
                new Driver(new string[] {"Driver", "Ted" }),
                new Driver(new string[] {"Driver", "Evan" })
            };
            var drivers = new List<Driver>();           
            foreach (var cmd in driverCmds)
                drivers.Add(cmd);

            //Act
            try
            {
                Driver.CheckForDuplicateDrivers(drivers);
                Assert.Fail("no exception thrown");
            }
            //Assert
            catch (InvalidDataException ex) {                                
                Assert.IsTrue(ex is InvalidDataException);
            }           
        }
        [Test]
        public void CanAssignTripsToDrivers()
        {
            //Arrange
            var trips = new List<Trip>()
            {
               new Trip(new string [] { "Trip", "Ted", "12:00", "14:00", "55.8" }),
               new Trip(new string [] { "Trip", "Ted", "05:00", "08:00", "200.8" }),
               new Trip(new string [] { "Trip", "Ted", "16:00", "18:00", "109.8" }),
               new Trip(new string [] { "Trip", "Bill", "16:00", "18:00", "109.8" })
            };
            var drivers = new List<Driver>() {
                (new Driver(new string[] { "Driver", "Ted" }))
            };

            //Act
            Driver.AssignTripsToDrivers(drivers, trips);

            //Assert
            Assert.IsTrue(drivers[0].Trips.Count == 3);
        }
        [Test]
        public void CanCalculateSpeedMph()
        {
            //Arrange
            var trip1 = new Trip(new string[] {"Trip", "Pepe", "12:00", "14:00", "55.8" });
            var trip2 = new Trip(new string[] {"Trip", "Ted", "12:00", "14:00", "109.8" });

            //Act
            var trip1Speed = Trip.CalculateSpeedMph(trip1);
            var trip2Speed = Trip.CalculateSpeedMph(trip2);

            Assert.AreEqual(27.9M, trip1Speed);
            Assert.AreEqual(54.9M, trip2Speed);
        }
        [Test]
        public void CanPopulateReportData()
        {
            //Arrange 
            var drivers = new List<Driver>() {
                (new Driver(new string[] { "Driver", "Ted" }))
            };
            var trips = new List<Trip>()
            {
               new Trip(new string [] { "Trip", "Ted", "12:00", "14:00", "55.8" }),
               new Trip(new string [] { "Trip", "Ted", "05:00", "08:00", "200.8" }),
               new Trip(new string [] { "Trip", "Ted", "16:00", "18:00", "109.8" })
            };
            foreach (var t in trips)
                drivers[0].Trips.Add(t);
                      
            //Act
            var data = ReportData.PopulateReportData(drivers);
            
            //Assert
            Assert.AreEqual("Ted", data[0].DriverName);
            Assert.AreEqual(366, data[0].TotalDistance);
            Assert.AreEqual(52, data[0].Speed);
        }
        [Test]
        public void CanSortReportDataByMileageDescending()
        {
            //Arrange 
            var reportDataList = new List<ReportData>()
            {
                new ReportData("Moe", 100, 50),
                new ReportData("Larry", 500, 70),
                new ReportData("Joe", 1000, 45)
            };
            //Act
            ReportData.SortReportDataByMileage(reportDataList);
            
            //Assert
            Assert.AreEqual("Joe", reportDataList[0].DriverName);
            Assert.AreEqual("Larry", reportDataList[1].DriverName);
            Assert.AreEqual("Moe", reportDataList[2].DriverName);
        }
        [Test]
        public void CanRoundDistance()
        {
            //Arrange 
            //Act
            //Assert
            Assert.AreEqual(1, ReportData.RoundDistance(0.5M));
            Assert.AreEqual(1, ReportData.RoundDistance(1.2M));
            Assert.AreEqual(4001, ReportData.RoundDistance(4000.8M));
        }
    }
}