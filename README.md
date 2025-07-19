# PackedTables.NET

[![NuGet Version](https://img.shields.io/nuget/v/PackedTables.Net)](https://www.nuget.org/packages/PackedTables.Net)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/download/dotnet/8.0)

A high-performance, serializable table structure library for .NET using MessagePack. Perfect for data caching, inter-process communication, and lightweight database scenarios.

## 🚀 Features

- **⚡ High Performance** - MessagePack binary serialization for speed and efficiency
- **🧵 Thread-Safe** - Built with `ConcurrentDictionary` for multi-threaded scenarios
- **💾 Multiple Formats** - Serialize to binary, Base64, JSON, or files
- **🎯 Type-Safe** - Comprehensive type system with automatic conversions
- **🔍 Queryable** - Navigate and search data with intuitive APIs
- **📊 Flexible Schema** - Dynamic column creation with validation support
- **🎮 Navigation** - Database-like cursor navigation through records

## 📦 Installation

```bash
dotnet add package PackedTables.Net
```

Or via Package Manager:
```powershell
Install-Package PackedTables.Net
```

## 🏁 Quick Start

### Creating Your First Table

```csharp
using PackedTables.Net;

// Create a new dataset
var dataSet = new PackedTableSet();

// Add a table with columns
var employeeTable = dataSet.AddTable("Employees");
employeeTable.AddColumn("Name", ColumnType.String);
employeeTable.AddColumn("Age", ColumnType.Int32);
employeeTable.AddColumn("Salary", ColumnType.Decimal);
employeeTable.AddColumn("HireDate", ColumnType.DateTime);

// Add some data
var row1 = employeeTable.AddRow();
row1["Name"].Value = "John Doe";
row1["Age"].Value = 30;
row1["Salary"].Value = 75000.50m;
row1["HireDate"].Value = DateTime.Now;

var row2 = employeeTable.AddRow();
row2["Name"].Value = "Jane Smith";
row2["Age"].Value = 28;
row2["Salary"].Value = 82000.00m;
row2["HireDate"].Value = DateTime.Now.AddMonths(-6);
```

### Navigation and Querying

```csharp
// Navigate like a database cursor
employeeTable.MoveFirst();
while (employeeTable.Current != null) {
    Console.WriteLine($"Employee: {employeeTable["Name"]} - Age: {employeeTable["Age"]}");
    if (!employeeTable.MoveNext()) break;
}

// Find specific records
if (employeeTable.FindFirst("Name", "John Doe")) {
    Console.WriteLine($"Found John, salary: {employeeTable["Salary"]}");
}

// Query with LINQ-style methods
var seniors = employeeTable.Where(row => row["Age"].Value.AsInt32() > 29);
var names = employeeTable.Select(row => row["Name"].Value.AsString());
```

### Serialization Options

```csharp
// Save to file (Base64 format)
dataSet.SaveToFile("employees.ptd");

// Load from file
var loadedDataSet = new PackedTableSet("employees.ptd");

// Export to JSON for debugging
string json = dataSet.SaveToJson();

// Serialize to Base64 string for transmission
string base64Data = dataSet.SaveToBase64String();

// Load from Base64
var newDataSet = new PackedTableSet();
newDataSet.LoadFromBase64String(base64Data);
```

## 📚 Core Concepts

### DataSet Structure
```
PackedTableSet
├── Tables (ConcurrentDictionary<int, TableModel>)
│   ├── Columns (ConcurrentDictionary<int, ColumnModel>)
│   └── Rows (ConcurrentDictionary<int, RowModel>)
│       └── Fields (ConcurrentDictionary<int, FieldModel>)
└── NameIndex (ConcurrentDictionary<string, int>)
```

### Supported Data Types

| ColumnType | .NET Type | Description |
|------------|-----------|-------------|
| `String` | `string` | Text data |
| `Int32` | `int` | 32-bit integers |
| `Int64` | `long` | 64-bit integers |
| `Decimal` | `decimal` | High-precision decimals |
| `DateTime` | `DateTime` | Date and time values |
| `Boolean` | `bool` | True/false values |
| `Guid` | `Guid` | Unique identifiers |
| `Bytes` | `byte[]` | Binary data |
| `Null` | `null` | Empty values |

## 🎯 Advanced Usage

### Table Management

```csharp
var dataSet = new PackedTableSet();

// Create multiple tables
var customers = dataSet.AddTable("Customers");
var orders = dataSet.AddTable("Orders");
var products = dataSet.AddTable("Products");

// Access tables by name
var customerTable = dataSet["Customers"];

// Check if table exists
if (dataSet.GetTableByName("Customers") != null) {
    // Table exists
}

// Remove a table
dataSet.RemoveTable("Customers");

// Get all tables
foreach (var table in dataSet.GetTables()) {
    Console.WriteLine($"Table: {table.Name}, Rows: {table.Rows.Count}");
}
```

### Working with Columns

```csharp
var table = dataSet.AddTable("Products");

// Add columns with specific types
var nameCol = table.AddColumn("ProductName", ColumnType.String);
var priceCol = table.AddColumn("Price", ColumnType.Decimal);
var categoryCol = table.AddColumn("Category", ColumnType.String);

// Column properties
nameCol.Rank = 1; // Display order
priceCol.Rank = 2;

// Remove a column (removes from all rows)
table.RemoveColumn(categoryCol.Id);

// Get column ID by name
int? nameColumnId = table.GetColumnID("ProductName");
```

### Row Operations

```csharp
var table = dataSet["Employees"];

// Add new row
var newEmployee = table.AddRow();
newEmployee["Name"].Value = "Alice Johnson";
newEmployee["Department"].Value = "Engineering";

// Access specific fields
var nameField = newEmployee["Name"];
Console.WriteLine($"Type: {nameField.ValueType}, Value: {nameField.Value}");

// Remove a row
table.RemoveRow(newEmployee);

// Row indexing and navigation
table.Reset(); // Go to first row
table.MoveNext(); // Move to next row
```

### Type Conversion Extensions

```csharp
// Safe type conversion methods
var ageField = row["Age"];
int age = ageField.AsInt32();
string ageString = ageField.Value.AsString();

// String parsing extensions
int number = "123".AsInt32();
DateTime date = "2023-12-25".AsDateTime();
decimal amount = "999.99".AsDecimal();

// Date formatting
DateTime now = DateTime.Now;
string dateStr = now.AsStrDate(); // "2023-12-25"
string timeStr = now.AsStrDateTime24H(); // "2023-12-25 14:30:45.123"
```

## 🏗️ Schema Validation (Upcoming)

Future versions will include comprehensive schema validation:

```csharp
// Schema definition (planned feature)
var column = table.AddColumn("Email", ColumnType.String);
column.IsRequired = true;
column.MaxLength = 255;
column.ValidationRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

// Validation
var result = table.ValidateRow(row);
if (!result.IsValid) {
    foreach (var error in result.Errors) {
        Console.WriteLine($"Validation Error: {error}");
    }
}
```

## 🎪 Real-World Examples

### Caching API Results

```csharp
// Cache web API data
var cache = new PackedTableSet();
var apiData = cache.AddTable("ApiResponses");
apiData.AddColumn("Endpoint", ColumnType.String);
apiData.AddColumn("Response", ColumnType.String);
apiData.AddColumn("Timestamp", ColumnType.DateTime);
apiData.AddColumn("StatusCode", ColumnType.Int32);

// Store cache to disk
cache.SaveToFile("api_cache.ptd");
```

### Configuration Storage

```csharp
// Application configuration
var config = new PackedTableSet();
var settings = config.AddTable("AppSettings");
settings.AddColumn("Key", ColumnType.String);
settings.AddColumn("Value", ColumnType.String);
settings.AddColumn("Category", ColumnType.String);

var row = settings.AddRow();
row["Key"].Value = "DatabaseConnection";
row["Value"].Value = "Server=localhost;Database=MyApp;";
row["Category"].Value = "Database";

// Serialize for storage
string configData = config.SaveToBase64String();
```

### Data Transfer Objects

```csharp
// Send data between services
var exportData = new PackedTableSet();
var customers = exportData.AddTable("Customers");
// ... populate data ...

// Efficient binary transfer
byte[] binaryData = Convert.FromBase64String(exportData.SaveToBase64String());
// Send binaryData over network/file/etc.

// Receive and deserialize
var importData = new PackedTableSet();
importData.LoadFromBase64String(Convert.ToBase64String(binaryData));
```

## ⚡ Performance Tips

1. **Use appropriate data types** - `Int32` vs `String` for numbers
2. **Batch operations** - Add multiple rows before serialization
3. **Index rebuilding** - Minimize column additions after data insertion
4. **Memory management** - Dispose of large datasets when done
5. **Async operations** - Use file I/O extensions for large datasets

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [MessagePack](https://github.com/neuecc/MessagePack-CSharp) for high-performance serialization
- Inspired by classic database table structures and modern .NET patterns

## 📞 Support

- 📖 [Documentation](https://github.com/mmeents/PackedTables.NET)
- 🐛 [Issue Tracker](https://github.com/mmeents/PackedTables.NET/issues)
- 💬 [Discussions](https://github.com/mmeents/PackedTables.NET/discussions)

---

**PackedTables.NET** - Making data serialization simple, fast, and reliable. ⚡