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

### File Persistence

Save and load datasets to/from files:


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
