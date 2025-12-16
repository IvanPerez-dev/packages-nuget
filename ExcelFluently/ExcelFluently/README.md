# ExcelFluently

ExcelFluently is a .NET library that allows you to import and export data from Excel files in a simple, fluid, and highly configurable way.
Its syntax is designed to be simple and coherent, making it easy to create Excel reports and map data from files without the need for complex configuration.


## Features
- Seamless Export: Generate Excel files from lists of objects with minimal or completely custom configuration.
- Flexible Import: Map Excel data to .NET objects by column name, index, or custom alias.
- Intuitive Configuration: Method chaining to adjust table styles, column names, date formats, and more.
- ASP.NET Core Compatible: Direct support for working with IFormFiles in controllers.


## Installation

Install the package via NuGet Package Manager or CLI:

```bash
dotnet add package ExcelFluently
```

## The package will automatically install these required dependencies:

- ClosedXML
- Microsoft.AspNetCore.Http.Features

# Quick Start

In this section, you'll find different ways to use ExcelFluently, both for exporting and importing data.

## Export data to Excel

### 1. Simple export (no configuration)
 Generates an Excel file automatically using the object's property names as columns:

```csharp
using ExcelFluently;

    public void ExportToExcel()
    {
        var users = _userRepository.GetAll();
        users.ToExcel().ToFile("C:\\Users\\Directory\\Desktop\\users.xlsx");
    }

```

### 2. Export with table style
Add a theme, colors, and row stripes:

```csharp

    public void ExportToExcel()
    {
        var users = _userRepository.GetAll();
        users.ToExcel()
            .WithTableStyle(configure =>
            {
                configure.Theme = ClosedXML.Excel.XLTableTheme.TableStyleMedium9;
                configure.ShowRowStripes = true;
                configure.HeaderFontColor = ClosedXML.Excel.XLColor.Black;
            })
            .ToFile("C:\\Users\\ISP2\\Desktop\\users.xlsx");
    }

```

### 3. Custom Columns

Rename columns or combine multiple properties into one:

```csharp
    public void ExportToExcel()
    {
     var users = _userRepository.GetAll();
     users.ToExcel()
            .WithTableStyle(configure =>
            {
                configure.Theme = ClosedXML.Excel.XLTableTheme.TableStyleMedium9;
                configure.ShowRowStripes = true;
                configure.HeaderFontColor = ClosedXML.Excel.XLColor.Black;
            })
            .WithColumn(x => x.Id, "Codigo")
            .WithColumn(x => x.Name + " " + x.Email, "Name")
            .WithColumn(x => x.DateOfBirth, "Fecha")
            .ToFile("C:\\Users\\ISP2\\Desktop\\users.xlsx");
    }
```
### 4. Date format
Applies a specific format to DateTime properties:

```csharp
    public void ExportToExcel()
    {
        var users = _userRepository.GetAll();
         users.ToExcel()
                .WithTableStyle(configure =>
                {
                    configure.Theme = ClosedXML.Excel.XLTableTheme.TableStyleMedium9;
                    configure.ShowRowStripes = true;
                    configure.HeaderFontColor = ClosedXML.Excel.XLColor.Black;
                })
                .WithColumn(x => x.Id, "Codigo")
                .WithColumn(x => x.Name + " " + x.Email, "Name")
                .WithColumn(x => x.DateOfBirth, "Fecha","yyyy/MM/dd")
                .ToFile("C:\\Users\\ISP2\\Desktop\\users.xlsx");
    }
```
### 5. Other configurations available

You can configure:

- Theme: Table theme (XLTableTheme)
- ShowRowStripes: Alternating rows
- HeaderFontColor: Header color
- ShowTotalsRow: Totals row
- Title: Report title
- SheetName: Sheet name

```csharp
    public void ExportToExcel()
    {
         var users = _userRepository.GetAll();
         users.ToExcel()
                .WithTableStyle(configure =>
                {
                    configure.Theme = ClosedXML.Excel.XLTableTheme.TableStyleMedium9;
                    configure.ShowRowStripes = true;
                    configure.HeaderFontColor = ClosedXML.Excel.XLColor.Black;
                    configure.ShowTotalsRow = true;
                    configure.Title = "Report of Users";
                    configure.SheetName = "Users";
                })....

    }
```

### Export as bytes
Useful for sending the file in an API:

```csharp
    ... 
    var bytes =  users.ToExcel().ToBytes();
    ...
```


## Import data from Excel to Objects

### 1. Simple import (by column name)
 The Excel file must have the same columns as the object properties:

``` csharp
    public List<User> MapusersFromExcelByNameColumn()
    {
         using var steam = File.OpenRead("C:\\Users\\Directory\\Desktop\\import users.xlsx");
            var users = steam
                .ImportExcel<User>(configure =>
                {
                    configure.SheetName = "Users";
                })
                .MapColumn(x => x.Name)
                .MapColumn(x => x.Email)
                .MapColumn(x => x.DateOfBirth)
                .MapColumn(x => x.Age)
                .MapColumn(x => x.Salary)
                .ToList();

            return users;
        }
   }
```

### 2. Import data from Excel with custom columns
 We can map the columns of the Excel file to the properties of the object

``` csharp
    public List<User> MapusersFromExcelByCustomColumn()
    {
         using var steam = File.OpenRead("C:\\Users\\Directory\\Desktop\\import users.xlsx");
            var users = steam
                .ImportExcel<User>(configure =>
                {
                    configure.SheetName = "Users";
                })
                .MapColumn(x => x.Name, "Full Name")
                .MapColumn(x => x.Email, "Email Address")
                .MapColumn(x => x.DateOfBirth, "DOB")
                .MapColumn(x => x.Age, "Age")
                .MapColumn(x => x.Salary, "Salary")
                .ToList();
            return users;
        }
   }
```

### 3. Import with date format
 Defines the expected format for dates:

 ``` csharp
    public List<User> MapusersFromExcelByCustomColumn()
    {
         using var steam = File.OpenRead("C:\\Users\\Directory\\Desktop\\import users.xlsx");
            var users = steam
                .ImportExcel<User>(configure =>
                {
                    configure.SheetName = "Users";
                    configure.DateFormat = "M/d/yyyy";
                })
                .MapColumn(x => x.Name, "Full Name")
                .MapColumn(x => x.Email, "Email Address")
                .MapColumn(x => x.DateOfBirth, "DOB")
                .MapColumn(x => x.Age, "Age")
                .MapColumn(x => x.Salary, "Salary")
                .ToList();
            return users;
        }
   }
```

### 4. Import by column index
 When there are no headers in the file:

``` csharp
    public List<User> MapusersFromExcelByIndexColumn()
    {
         using var steam = File.OpenRead("C:\\Users\\Directory\\Desktop\\import users.xlsx");
            var users = steam
                .ImportExcel<User>(configure =>
                {
                    configure.SheetName = "Users";
                })
                .MapColumn(x => x.Name, 0)
                .MapColumn(x => x.Email, 1)
                .MapColumn(x => x.DateOfBirth, 2)
                .MapColumn(x => x.Age, 3)
                .MapColumn(x => x.Salary, 4)
                .ToList();
            return users;
        }
   }
```

### 5. Use in ASP.NET Core
Import data directly from an uploaded file:

``` csharp
    [HttpPost("import")]
    public IActionResult ImportUsers(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var users = file
            .ImportExcel<User>(configure =>
            {
                configure.SheetName = "Users";
            })
            .MapColumn(x => x.Name, "Full Name")
            .MapColumn(x => x.Email, "Email Address")
            .MapColumn(x => x.DateOfBirth, "DOB")
            .MapColumn(x => x.Age, "Age")
            .MapColumn(x => x.Salary, "Salary")
            .ToList();
        // Process the imported users as needed
        return Ok(users);
    }
```

# Benefits

- Fluent and readable syntax.
- Minimal configuration for simple cases.
- Highly customizable for complex scenarios.
- Ideal for reports, data migrations, and APIs.

# Dependencies

- This package automatically includes:
- ClosedXML (0.105.0)
- Microsoft.AspNetCore.Http.Features (5.0.17)


# License
MIT License

# Support

- GitHub: [https://github.com/IvanPerez-dev](https://github.com/IvanPerez-dev)
- LinkedIn: [https://www.linkedin.com/in/ivan-perez-tintaya/](https://www.linkedin.com/in/ivan-perez-tintaya/)
