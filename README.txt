Environment and Building the App (Win 10 machine):

After cloning RootApp.bundle to a local directory, open the RootApp.sln file on a Windows 10
machine with Visual Studio 2017 or later installed (Enterprise, Professional, or Community Edition).  
If the machine does not have Visual Studio installed, it 
can be found at https://visualstudio.microsoft.com/vs/community/ 
Make sure .NET Core 2.2 or later is included with the Visual Studio installation.
.NET Core 2.2 is the target framework for the application.

**Visual Studio can be installed on macOS and .NET Core is cross-platform,
so this application will probably work on mac, although it has not been tested on macOS.

To build the app, Right-click on the Root project icon in Visual Studio solution explorer 
(solution explorer can be accessed by clicking on view -> solution explorer) and select "Publish".
When asked to create a publish target select "folder" and click create profile.

The following options should be set as follows in the Publish Summary Section:

Configuration: Release|Any CPU
Target Framework: netcoreapp2.2
Deployment Mode: Framework Dependent
Target Runtime: win-x64
Target Location : This can be whatever you want

Click the "Publish" button after configuring options.

Navigate to the target location where the app was published.  Open a command line in the target directory 
and type "Root.exe input.txt" into the command line. The app should run, producing a Report folder in the 
the target directory containing the report (report.txt). 

The tests inside the RootTest project can be verified by right-clicking on the project icon in solution
explorer in Visual Studio and then selecting "Run Tests".

***********************************************************************************************************

I am most familiar with C#/.NET from my work experience and education.
I elected to use a .NET Core console application to solve the problem.

Overview of the Root console application:

1) The app accepts a command line argument specifying the name of the input file
	The file is expected to be in a folder relative to the working directory and 
	is verified to exist in the expected location.	
2) A StreamReader object reads the data and places it into a list line by line, 
   ignoring any empty lines or lines with only whitespace.
3) The app attempts to parse commands from each raw input line and creates a Trip or Driver
   object if successful. Validation of the command is done with a method named IsValidData()
   inside of the object constructor.  If the data attached to a command is not valid then its
   isValid property is set to false and the bad command is added to a list that 
   will be printed to the console with a warning message.   
   
   IMO, using objects is the most intuitive way to implement a solution to the problem statement.   
   The Trip and Driver classes both inheirit from an abstract base class called "RootCommand".
   Children of RootCommand will all have an IsValid property and must implement an IsValidData() method.  
   I considered using an IRootCommand interface here instead but I wanted to have a couple of general
   command methods with an actual implementation.

4) Commands are grouped by type.
5) The list of Driver command objects is checked for duplicate names.
6) Trips are assigned to the corresponding Driver.
7) ReportData is calculated for each Driver (total distance, average speed)
8) A report.txt file is generated using the ReportData calculated from each Driver's Trips.
   

*** The app will fail if unknown commands are found or if duplicate drivers are detected.  
If one/both of these scenarios occurs, there is a significant problem with the 
input data and the program should halt, IMO.  Duplicate drivers could actually be two different people 
and not just invalid data, so trip assignment would be ambiguous.  












