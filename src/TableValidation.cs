using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PackedTables.Net {

  public enum TableState {
    Browse,    // Just viewing/navigating
    Edit,      // Editing existing row
    Insert     // Adding new row (not yet committed)
  }

  // Validation classes
  public class ValidationResult {
    public bool IsValid { get; set; } = true;
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();

    public bool HasErrors => Errors.Count > 0;
    public bool HasWarnings => Warnings.Count > 0;

    public void AddError(string message) {
      IsValid = false;
      Errors.Add(message);
    }

    public void AddError(string columnName, string message) {
      IsValid = false;
      Errors.Add($"{columnName}: {message}");
    }

    public void AddWarning(string message) {
      Warnings.Add(message);
    }

    public void AddWarning(string columnName, string message) {
      Warnings.Add($"{columnName}: {message}");
    }
  }

  public static class TableValidation {
    public static ValidationResult ValidateRow(this TableModel table, RowModel row) {
      var result = new ValidationResult { IsValid = true };

      foreach (var column in table.Columns.Values) {
        ValidateField(table, row, column, result);
      }

      return result;
    }

    public static void ValidateField(TableModel table, RowModel row, ColumnModel column, ValidationResult result) {
      // Skip validation for non-visible columns if desired
      if (!column.IsVisible) return;

      try {
        var field = row[column.ColumnName];
        var value = field.Value;

        // Required field validation
        if (column.IsRequired && IsNullOrEmpty(value)) {
          result.AddError(column.ColumnName, "Field is required");
          return;
        }

        // Skip other validations if field is empty and not required
        if (IsNullOrEmpty(value)) return;

        // Type validation
        if (!ValidateType(value, column.ColumnType)) {
          result.AddError(column.ColumnName,
              $"Value '{value}' is not valid for type {column.ColumnType}");
        }

        // Length validation for strings
        if (column.ColumnType == ColumnType.String && column.MaxLength > 0) {
          var stringValue = value?.ToString() ?? "";
          if (stringValue.Length > column.MaxLength) {
            result.AddError(column.ColumnName,
                $"Value exceeds maximum length of {column.MaxLength}");
          }
        }

        // Regex validation
        if (!string.IsNullOrEmpty(column.ValidationRegex) && value != null) {
          if (!Regex.IsMatch(value.ToString() ?? "", column.ValidationRegex)) {
            result.AddError(column.ColumnName,
                $"Value does not match required pattern");
          }
        }

        // Range validation for numeric types
        ValidateNumericRange(column, value, result);

      } catch (Exception ex) {
        result.AddError(column.ColumnName, $"Validation error: {ex.Message}");
      }
    }

    private static bool IsNullOrEmpty(object? value) {
      return value == null ||
             (value is string str && string.IsNullOrWhiteSpace(str)) ||
             (value.ToString() == "");
    }

    private static bool ValidateType(object value, ColumnType expectedType) {
      try {
        byte[]? bytes = null;
        return expectedType switch {
          ColumnType.String => true, // Any value can be a string
          ColumnType.Int32 => int.TryParse(value.ToString(), out _),
          ColumnType.Int64 => long.TryParse(value.ToString(), out _),
          ColumnType.Decimal => decimal.TryParse(value.ToString(), out _),
          ColumnType.DateTime => DateTime.TryParse(value.ToString(), out _),
          ColumnType.Boolean => bool.TryParse(value.ToString(), out _),
          ColumnType.Guid => Guid.TryParse(value.ToString(), out _),
          ColumnType.Bytes => value is byte[] || Convert.TryFromBase64String(value.ToString() ?? "", bytes, out _),
          _ => true
        };
      } catch {
        return false;
      }
    }

    private static void ValidateNumericRange(ColumnModel column, object value, ValidationResult result) {
      // You could add MinValue/MaxValue properties to ColumnModel for this
      // For now, just basic overflow checking
      try {
        switch (column.ColumnType) {
          case ColumnType.Int32:
            Convert.ToInt32(value);
            break;
          case ColumnType.Int64:
            Convert.ToInt64(value);
            break;
          case ColumnType.Decimal:
            Convert.ToDecimal(value);
            break;
        }
      } catch (OverflowException) {
        result.AddError(column.ColumnName, $"Value is outside valid range for {column.ColumnType}");
      }
    }
  }

  // Event argument classes
  public class RowValidationEventArgs : EventArgs {
    public RowModel Row { get; }
    public ValidationResult Result { get; }
    public bool Cancel { get; set; } = false;

    public RowValidationEventArgs(RowModel row, ValidationResult result) {
      Row = row;
      Result = result;
    }
  }

  public class FieldValidationEventArgs : EventArgs {
    public RowModel Row { get; }
    public string ColumnName { get; }
    public FieldModel Field { get; }
    public ValidationResult Result { get; }
    public bool Cancel { get; set; } = false;

    public FieldValidationEventArgs(RowModel row, string columnName, ValidationResult result) {
      Row = row;
      ColumnName = columnName;
      Field = row[columnName]; // Get the actual field
      Result = result;
    }
  }

  public class ValidationEventArgs : EventArgs {
    public ValidationResult Result { get; }
    public string? Context { get; set; } // Optional context info

    public ValidationEventArgs(ValidationResult result, string? context = null) {
      Result = result;
      Context = context;
    }
  }

  public class StateChangedEventArgs : EventArgs {
    public TableState OldState { get; }
    public TableState NewState { get; }
    public StateChangedEventArgs(TableState oldState, TableState newState) {
      OldState = oldState;
      NewState = newState;
    }
  }


  // Other code...

  [Serializable]
  public class ValidationException : Exception {
    public ValidationResult? ValidationResult { get; }
    public List<string> ValidationErrors { get; }
    public List<string> ValidationWarnings { get; }

    public ValidationException() : base("Validation failed") {
      ValidationErrors = new List<string>();
      ValidationWarnings = new List<string>();
    }

    public ValidationException(string message) : base(message) {
      ValidationErrors = new List<string>();
      ValidationWarnings = new List<string>();
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException) {
      ValidationErrors = new List<string>();
      ValidationWarnings = new List<string>();
    }

    public ValidationException(ValidationResult validationResult)
        : base(CreateMessage(validationResult)) {
      ValidationResult = validationResult;
      ValidationErrors = new List<string>(validationResult.Errors);
      ValidationWarnings = new List<string>(validationResult.Warnings);
    }

    public ValidationException(string message, ValidationResult validationResult)
        : base(message) {
      ValidationResult = validationResult;
      ValidationErrors = new List<string>(validationResult.Errors);
      ValidationWarnings = new List<string>(validationResult.Warnings);
    }

    private static string CreateMessage(ValidationResult result) {
      if (result.HasErrors) {
        return $"Validation failed with {result.Errors.Count} error(s): {string.Join("; ", result.Errors)}";
      }
      return "Validation failed";
    }

    public override string ToString() {
      var baseString = base.ToString();
      if (ValidationErrors.Count > 0) {
        baseString += $"\nValidation Errors: {string.Join(", ", ValidationErrors)}";
      }
      if (ValidationWarnings.Count > 0) {
        baseString += $"\nValidation Warnings: {string.Join(", ", ValidationWarnings)}";
      }
      return baseString;
    }
  }

}
