using System.Collections.Generic;
using System.IO;

namespace RootData.Models
{
    public abstract class RootCommand
    {
        public bool IsValid { get; protected set; }
        protected abstract bool IsValidData(string[] data);

        public static RootCommand ParseCommand(string data, string delimiter)
        {
            var command = data.Trim().Split(delimiter);
            var cmdType = command[0].ToUpper();
            RootCommand rootCmd;            

            switch (cmdType)
            {
                case "DRIVER":
                    rootCmd = new Driver(command);
                    break;
                case "TRIP":
                    rootCmd = new Trip(command);
                    break;
                default:
                    throw new InvalidDataException($"Invalid command type found: {command[0]}");
            }
            return rootCmd;            
        }
        public static void GroupCommandsByType(List<RootCommand> commands, List<Driver> drivers, List<Trip> trips)
        {
            foreach (var cmd in commands)
            {
                var cmdType = cmd.GetType();
                
                if (cmdType == typeof(Driver))
                    drivers.Add((Driver)cmd);
                else if (cmdType == typeof(Trip))
                    trips.Add((Trip)cmd);
            }            
        }
    }
}
