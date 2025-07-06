# Overview of PackedTables.NET

**PackedTables.NET** is a lightweight, in-memory data table framework designed for managing tabular data in a structured, strongly-typed manner. It supports multiple tables within a dataset, with each table containing rows, columns, and fields. The framework leverages the MessagePack serialization library for efficient data serialization and deserialization, making it suitable for applications requiring compact data storage or transmission.

---

## Key Features

- **Data Structure:**  
  Hierarchical model with:
  - `DataSetModel` (collection of tables)
  - `TableModel` (rows and columns)
  - `RowModel` (fields)
  - `FieldModel` (individual values with type information)

- **Type Safety:**  
  Uses a `ColumnType` enum to enforce type constraints for field values (e.g., `Int32`, `String`, `DateTime`, etc.).

- **Concurrency:**  
  Utilizes `ConcurrentDictionary` for thread-safe operations on tables, rows, and fields.

- **Serialization:**  
  Supports saving/loading data to/from Base64 strings or files using MessagePack.

- **Indexing:**  
  Maintains indexes for efficient column and row lookups, with support for sorting and ordering.

- **Ownership Model:**  
  Establishes parent-child relationships (e.g., `TableModel` owns `RowModel`, `RowModel` owns `FieldModel`) to ensure data consistency.

---

## Primary Entry Point

The main entry point is the `PackedTableSet` class, which manages a collection of `TableModel` instances and provides methods for creating, modifying, and serializing tables.

---

# Code Review

## Strengths

- **Clear Structure:**  
  The hierarchical design (`DataSetModel` → `TableModel` → `RowModel` → `FieldModel`) is intuitive and mirrors traditional relational database concepts, making it easy to understand.

- **Type Safety:**  
  The `ColumnType` enum and `FieldModel.Value` property ensure that field values are validated against their declared types, reducing runtime errors.

- **Concurrency Support:**  
  Using `ConcurrentDictionary` for columns, rows, and row fields enables thread-safe operations, which is critical for multi-threaded applications.

- **Efficient Serialization:**  
  Integration with MessagePack ensures compact and fast serialization/deserialization, ideal for saving to files or transmitting over networks.

- **Index Management:**  
  The `RebuildColumnIndex` and `RebuildRowIndex` methods maintain efficient lookups and sorting, improving performance for large datasets.

- **Ownership Tracking:**  
  The `Owner` properties maintain relationships between models, ensuring that modifications propagate correctly (e.g., setting `Modified = true` on the parent `PackedTableSet`).

- **Comprehensive Type Handling:**  
  The class supports all `ColumnType` values (`Null`, `Boolean`, `Int32`, `Int64`, `Decimal`, `DateTime`, `Guid`, `String`, `Bytes`) with methods like `GetColumnType`, `GetValueString`, and `AsObject`, ensuring consistent type conversions.

- **Date Formatting:**  
  The date-to-string methods (`AsStrDate`, `AsStrDateTime24H`, etc.) use `CultureInfo.InvariantCulture` for consistent, culture-independent formatting, which is ideal for serialization.

- **Extension Methods:**  
  The use of extension methods (e.g., `AsInt32`, `AsDateTime`) simplifies type conversions for `FieldModel` and raw strings/objects, improving code readability.

- **File I/O:**  
  The async `ReadAllTextAsync` and `WriteAllTextAsync` methods align with modern .NET practices, supporting non-blocking file operations.

---

## Areas for Improvement

### Error Handling

- **Parsing Errors:**  
  Methods like `AsInt32`, `AsDateTime`, and `AsDecimal` use `Parse` without handling invalid input. For example, `int.Parse(value)` in `AsInt32` will throw a `FormatException` if the value is not a valid integer.  
  **Recommendation:** Use `TryParse` to return default values or throw custom exceptions for clarity and safety.

- **Generic Exceptions:**  
  The `GetColumnValueToString` and `GetValueString` methods throw generic `Exception` objects. Use a custom `PackedTableException` for better error specificity.

- **Null Handling:**  
  Methods like `AsObject` return an empty string (`""`) for `ColumnType.Null` and `ColumnType.Unknown`. Consider returning `null` for `Null` to align with .NET conventions.

### Type Conversion Robustness

- **Limited Conversion Flexibility:**  
  The `FieldModel.Value` setter only allows `int.TryParse` for `Int32` conversions. Add support for other types (e.g., `Decimal.TryParse`, `DateTime.TryParse`) to allow flexible conversions where possible.

- **Bytes Handling:**  
  The `AsBase64Bytes` method assumes the input string is Base64-encoded, but `FromUTF8AsBytes` and `FromUTF8BytesAsString` use UTF-8 encoding. Clarify whether `ColumnType.Bytes` is meant for Base64 or UTF-8 and ensure consistency. For example, if `Bytes` is for arbitrary binary data, use Base64 consistently:
- ```csharp
  public static byte[] FromBase64String(string base64) {
    return Convert.FromBase64String(base64);
  }
  ````
- **Guid Parsing:**  
  The `AsObject` method uses `Guid.Parse` without `TryParse`, which could throw a `FormatException`. Use `Guid.TryParse` for safer parsing.

### Performance

- **String Conversions:**  
  Repeatedly converting values to/from strings (e.g., `ValueString` to `int` in `AsInt32`) incurs parsing overhead. Consider storing native types in `FieldModel` (e.g., a private `object _value`) and using `ValueString` as a computed property for serialization.

```csharp
[MessagePackObject]
public class FieldModel {
    [Key(3)] public string ValueString { get; set; } = "";
    [Key(4)] public ColumnType ValueType { get; set; } = ColumnType.Null;
    [IgnoreMember] private object? _nativeValue;
    [IgnoreMember]
    public object Value {
        get => _nativeValue ?? AsObject();
        set {
            ValueType = GetColumnType(value);
            _nativeValue = value;
            ValueString = GetValueString(value);
        }
      }
  } 
  ```
- **Redundant Conversions:**  
  The `AsObject` method always returns an empty string for `ColumnType.String`, even if `ValueString` is non-empty. Simplify the logic:case ColumnType.String: ret = field.ValueString ?? ""; break;
### Code Organization

- **Missing Documentation:**  
  Most methods lack XML comments, making it harder for users to understand their purpose. Add XML documentation, especially for public methods.

- **Inconsistent Naming:**  
  Method names like `FromUTF8AsBytes` and `FromUTF8BytesAsString` are verbose. Consider shorter names like `ToUTF8Bytes` and `FromUTF8Bytes`.

- **Duplicate Methods:**  
  There are separate `AsInt32` methods for `string`, `FieldModel`, and `object`. Consolidate these using a single implementation where possible:
 ```csharp
  public static int AsInt32(this object value) {
    return value is string s ? int.Parse(s) : Convert.ToInt32(value);
  }
 ```
### File I/O

- **Consolidation:**  
  The `ReadAllTextAsync` and `WriteAllTextAsync` methods are duplicated in `PackedTableSet` (as synchronous calls) and `FieldExt` (as async). Consolidate these in `FieldExt` and update `PackedTableSet` to use the async versions.

- **Return Type:**  
  The `WriteAllTextAsync` return type (`Task<int>`) is unusual, returning a constant `1`. Return `Task` instead for simplicity:
- ```csharp
  public static async Task WriteAllTextAsync(this string content, string fileName) {
    using var streamWriter = new StreamWriter(fileName);
    await streamWriter.WriteAsync(content);
  }
  ```
### Edge Cases

- **Empty Strings:**  
  Methods like `AsDateTime` or `AsDecimal` don’t handle empty strings explicitly. Add checks to return default values or throw meaningful errors.

- **Culture Sensitivity:**  
  While `AsStrDateTime24H` uses `CultureInfo.InvariantCulture`, ensure all parsing methods (e.g., `AsDateTime`) also use invariant culture to avoid locale-specific issues.

- **Unknown Types:**  
  The `GetColumnType` and `GetValueString` methods handle `ColumnType.Unknown` inconsistently. Consider throwing an exception or logging a warning for unknown types.

