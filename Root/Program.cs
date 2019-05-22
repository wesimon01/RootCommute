using Microsoft.Extensions.Configuration;
using RootData.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Root
{
    public class Program
    {        
        private static List<Driver> drivers = new List<Driver>();
        private static List<Trip> trips = new List<Trip>();
        private static List<ReportData> reportData = new List<ReportData>();
        private static List<RootCommand> cmds = new List<RootCommand>();
        private static List<string> invalidCmds = new List<string>();
        private static string workingDir;

        static void Main(string[] args)
        {    
            try
            {     
                #if DEBUG          
                    workingDir = Directory.GetParent(Directory.GetParent
                                 (Directory.GetCurrentDirectory()).Parent.FullName).FullName;                   
                #else                
                    workingDir = Directory.GetCurrentDirectory();               
                #endif

                #region Initialize Config               
                var builder = new ConfigurationBuilder()
                .SetBasePath(workingDir)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfigurationRoot config = builder.Build();               
                #endregion Initialize Config
                
                ProgramUtil.ValidateArgs(args, Path.Combine(workingDir, config["inputFolder"]));  
                string inputFilePath = Path.Combine(workingDir, config["inputFolder"], args[0]);
                
                Console.WriteLine($"Found input file: {inputFilePath}\n");                               
                Console.WriteLine("Generating Driver Report\n");                
                List<string> rawInput = ProgramUtil.GetRawInput(inputFilePath);
                                
                foreach (var s in rawInput) {     
                    var cmd = RootCommandUtil.ParseCommand(s, config["delimiter"]);    
                    if (cmd.IsValid) 
                        cmds.Add(cmd);
                    else 
                        invalidCmds.Add(s); 
                }
                RootCommandUtil.GroupCommandsByType(cmds, drivers, trips);

                Driver.CheckForDuplicateDrivers(drivers);                
                trips = Trip.FilterTripsBySpeed(trips);                
                Driver.AssignTripsToDrivers(drivers, trips);
                
                reportData = ReportData.PopulateReportData(drivers);
                ReportData.SortReportDataByMileage(reportData);

                string outputPath = Path.Combine(workingDir, config["outputFolder"]);
                ReportData.WriteReport(reportData, outputPath, config["outputFile"]);                              
            }           
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            if (invalidCmds.Count > 0) {
                ProgramUtil.ConsoleColorWriteLine($"!WARNING - INVALID COMMANDS!", ConsoleColor.Yellow);               
                foreach(var cmd in invalidCmds) { 
                    ProgramUtil.ConsoleColorWriteLine($"Invalid Command found: {cmd}", ConsoleColor.Yellow);
                }
            }
            ProgramUtil.ConsoleColorWriteLine("COMPLETE!\n", ConsoleColor.Green);           
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            //TODO: log exceptions, log invalid commands?
        }            
    }
}
