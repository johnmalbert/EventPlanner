**JQZ Event Planner** -- 
**Start planning events with a few clicks**

This app was built in C# using Asp.NET. With dotnet installed on your machine, Here's how you can get the app  up and running: 
![event_planner](https://user-images.githubusercontent.com/24249474/114450168-8723e200-9b8a-11eb-82af-211a7706737e.gif)


```
 git clone https://github.com/johnmalbert/EventPlanner.git
```

This app uses MySQL as a database. You can check whether it is installed on your machine by typing 

```
mysql -u root -p
```

If you don't get through, you will need to install MySQL. Visit this website for download and instructions:

https://dev.mysql.com/downloads/mysql/

Once MySQL is installed, here's what you will need to run to connect to your database:

1. Create an appsettings.json file on the project level of the application. 
2. In appsettings.json, enter the following code, substituting your password, port (commonly 3306), and database name

```
{
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "AllowedHosts": "*",
      "DBInfo":
      {
          "Name": "MySQLconnect",
          "ConnectionString": "server=localhost;userid=root;password=YOUR_PW;port=YOUR_PORT;database=DB_NAME;SslMode=None"
      }
  }
```
3. Save appsettings.json.

4. This app uses google maps to show event locations. If you have your own API key, create a file on the project level called Keys.cs

5. Save this into Keys.cs, substituting your API key.
```
namespace EventPlanner
{
    public class Keys
    {
        public static string MapsKey { get; set; } = "YOUR_API_KEY";
    }
}
```
NOTE: If you don't have your own API Key or don't need a map on the app, you can remove lines 16-34 in EventPlanner>Views>DisplayEvent.cs

6. If you don't have Entity Framework Core installed, run 
```
dotnet tool install --global dotnet-ef
```
7. Run the following commands to install the Pomelo package and Entity Framework Core, which connect the app to MySQL
```
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 3.1.1
dotnet add package Microsoft.EntityFrameworkCore.Design --version 3.1.5
dotnet ef migrations add FirstMigration
dotnet ef database update
```
8. Finally, you are good to go! Run this on the project level (same as Program.cs)
```
dotnet run
```
9. Open up localhost:5000 and start creating events! You're done!
