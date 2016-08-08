# MULE
## Magical Unicorn Land (Enterprise Edition)
![Unikitty Loves MULE](https://github.com/badcommandorfilename/mule/blob/master/muleapp/wwwroot/img/unikitty.png)

C# NET Core MVC Web UI for CRUD actions on a simple collection of objects

## Wow!

MULE is just a simple Web UI that wraps up Create, Read, Update and Delete actions on an SQLite database.

Have you ever had a conversation that goes like this?
>  "Well, in an ideal world, with magic and unicorns, we'd have a cool web portal where people could log in and update which servers they were using and what version of the software they were running! Until then, I guess we'll all just put post-it notes everywhere and track everything on this ugly whiteboard."

## Such Magic!

You can use MULE as a seed app to build your own dashboards. Create your datamodel class as shown below, and the view engine will magically update the GUI for you! 

```csharp
    public class AppHost
    {
        [Key]//Unique id for object
        public string URL { get; set; } = "";

        string version = "";
        [Required] //Property cannot be null
        public string Version { get { return version; } set { version = value ?? ""; } }

        [DataType(DataType.MultilineText)] //Input style/formatting hint
        public string Description { get; set; } = "";

        [HiddenInput(DisplayValue = false)] //Property won't show in edit form
        public DateTime Updated { get; private set; } = DateTime.Now;

        [HiddenInput(DisplayValue = false)] //Property won't show in edit form
        [NotMapped] //Don't store this value in DB, calculate or set in controller
        public int RTT { get; set; }

        /// Equality Comparer (determines if DB row will be overwritten or created)
        public override bool Equals(object obj) => URL == (obj as AppHost)?.URL;

        /// Unique key definition (use primary key)
        public override int GetHashCode() => URL.GetHashCode();
    }
```

The unicorns will use [EF7](https://github.com/aspnet/EntityFramework) and [SQLite](https://www.nuget.org/packages/EntityFramework.SQLite/) to persist your data.

## Unicorns Exist!

MULE uses C# & .NET Core and should run on:
* Windows 7, 8, 10...
* Linux (Ubuntu 14.04, 16.04, Linux Mint 17+, Debian 8+, Fedore 23+...)
* Mac OS X 10+
* If one of those doesn't work, use [Docker](https://github.com/badcommandorfilename/mule/blob/master/muleapp/Dockerfile)!

## Install
* Install/Update [.NET Core](https://www.microsoft.com/net/core#ubuntu)
* Checkout repo
* Open the "muleapp" directory
* run "dotnet restore"
* run "dotnet run"

## Troubleshooting
If you change your datamodel, don't forget to use:
* dotnet ef migrations add "Schema Change"
* dotnet ef database update
