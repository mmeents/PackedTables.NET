# PackedTables.NET

PackedTables.NET is a high-performance, lightweight library for managing and serializing table-based data structures using [MessagePack](https://github.com/neuecc/MessagePack-CSharp). It is designed to handle complex data relationships efficiently while maintaining a compact serialized format, making it ideal for scenarios where performance and data size are critical.

## Features

- **Table-Based Data Management**: Provides a structured way to manage tables, rows, columns, and fields.
- **High-Performance Serialization**: Leverages MessagePack for fast and compact serialization.
- **Base64 and JSON Support**: Save and load data in Base64 or JSON formats for interoperability.
- **File Persistence**: Easily save and load datasets to and from files.
- **Extensibility**: Includes extension methods for common operations like serialization, deserialization, and file I/O.
- **.NET 8 Support**: Built with modern .NET features for maximum performance and compatibility.

## Installation

PackedTables.NET is available as a NuGet package. You can install it using the .NET CLI:

Or via the NuGet Package Manager in Visual Studio.

## Getting Started

### Basic Usage
To get started with PackedTables.NET, you can create a simple table and perform basic operations like adding rows and columns:
```csharp
  var packedTables = new PackedTableSet();  -- Create a new PackedTableSet instance
  
  packedTables.LoadFromFile("path/to/your/file.pt"); -- maybe load from a previous save
  var settings = packedTables["Settings"];  -- Retrieve the "Settings" table if it exists
  if (settings == null) {  -- If the table does not exist, create it
    settings = packedTables.AddTable("Settings");
    settings.AddColumn("Key", ColumnType.String);
    settings.AddColumn("Value", ColumnType.String);
  }

  var aRow = settings.AddRow();  -- Add a new row to the "Settings" table
  aRow["Key"].Value = "TestKey1";
  aRow["Value"].Value = "TestValue1";

  var aRow2 = settings.AddRow(); -- Add another row to the "Settings" table
  aRow2["Key"].Value = "TestKey2";
  aRow2["Value"].Value = "TestValue2";

  var retrievedSettings = packedTables["Settings"];  -- Retrieve the "Settings" table again

  Assert.IsNotNull(retrievedSettings, 
    "Settings table should not be null after saving.");
  Assert.IsTrue(retrievedSettings.Columns.Count == 2, 
    "Settings table should have 2 columns after saving Key and Value.");
  Assert.AreEqual(2, retrievedSettings.Rows.Count, 
    "Settings table should have 2 rows after saving.");

  string packedData = packedTables.SaveToBase64String();

  var anotherSet = new PackedTableSet();
  anotherSet.LoadFromBase64String(packedData);

  var settings2 = anotherSet["Settings"];
  settings2.FindFirst("Key", "TestKey2");
  Assert.IsTrue(settings2.CurrentRow != null, 
    "Current row should not be null after finding TestKey2.");
  Assert.AreEqual("TestValue2", 
    settings2.Current["Value"].Value, 
    "Value for TestKey2 should be TestValue2 after finding.");



```

### File Persistence

Save and load datasets to/from files:

### Checkout PackedTables.Viewer App
PackedTables.NET includes a simple viewer application that allows you to visualize and interact with your packed tables. You can find the viewer in the `PackedTables.Viewer` project within the solution. This application provides a user-friendly interface to explore your data, making it easier to understand and manage complex datasets.

## Contributing

Contributions are welcome! If you’d like to contribute, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Commit your changes with clear and concise messages.
4. Submit a pull request.

Please ensure your code adheres to the existing coding style and includes appropriate tests.

## License

PackedTables.NET is licensed under the [MIT License](LICENSE). You are free to use, modify, and distribute this software in accordance with the license terms.

## Acknowledgments

PackedTables.NET is built on top of the excellent [MessagePack-CSharp](https://github.com/neuecc/MessagePack-CSharp) library. Special thanks to its contributors for their work on high-performance serialization.
