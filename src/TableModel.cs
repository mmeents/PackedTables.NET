using MessagePack;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace PackedTables.Net
{
  [MessagePackObject(AllowPrivate = true)]
  public class TableModel : IEnumerator<RowModel>, IQueryableTable {

    [IgnoreMember]
    private PackedTableSet? _owner = null;

    /// <summary>
    /// owner gets set after deserialization so including to build indexes as part of the deserialization process.
    /// </summary>
    [IgnoreMember ]
    public PackedTableSet? Owner {
      get{ return _owner; }
      set { 
        _owner = value;
        RebuildColumnIndex();
        RebuildRowIndex();
      } 
    }

    /// <summary>
    /// Dispose is implemented to satisfy IEnumerator&lt;T&gt; but has no cleanup logic.
    /// TableModel manages its own state and doesn't hold unmanaged resources.
    /// </summary>
    public void Dispose() {
      // No cleanup needed - this class doesn't hold unmanaged resources
      // Current iterator state is reset in Reset() method
    }

    /// <summary>
    /// Id is lookup key in the Tables dictionary of the DataSetModel.
    /// </summary>
    [Key(0)]
    public int Id { get; set; } = 0;

    /// <summary>
    /// Value for index, Table Name must be unique.
    /// </summary>
    [Key(1)]
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the collection of the tables columns, dictionary indexed by ColumnModels Id field.
    /// </summary>
    [Key(2)]
    public ConcurrentDictionary<int, ColumnModel> Columns { get; set; } = new();

    /// <summary>
    /// Gets or set the column name to Id index, used for direct lookup of column Id by column name.
    /// </summary>
    [IgnoreMember]
    private ConcurrentDictionary<string, int> _columnNameToIdIndex { get; set; } = new();

    /// <summary>
    /// Gets or sets the column order by rank and visiblity to Id index, used for direct lookup of column Id by column number 0 baised.
    /// </summary>
    [IgnoreMember]
    private ConcurrentDictionary<int, int> _columnOrderByRankToIdIndex = new();

    [IgnoreMember]
    private TableState _tableState = TableState.Browse;

    [IgnoreMember]
    public TableState State { 
      get { return _tableState;} 
      private set { 
        if (_tableState == value) return; // No change, no event
        var oldState = _tableState;
        _tableState = value; 
        StateChanged?.Invoke(this, new StateChangedEventArgs(oldState, _tableState));
      } 
    }

    public event EventHandler<StateChangedEventArgs>? StateChanged;

    [IgnoreMember]
    private RowModel? _editingRow; // Track row being edited or inserted

    public void RebuildColumnIndex() {
      _columnNameToIdIndex.Clear();
      _columnOrderByRankToIdIndex.Clear();
      var listCols = Columns.Select(x => x.Value).OrderBy(x => x.Rank);
      int visualIndex = 0;
      foreach (var column in listCols) {
        if (!string.IsNullOrEmpty(column.ColumnName)) {
          _columnNameToIdIndex[column.ColumnName] = column.Id;
        }
        if (column.IsVisible) {
          _columnOrderByRankToIdIndex[visualIndex] = column.Id;
        }        
        visualIndex++;
      }
    }

    public ColumnModel AddColumn(string columnName, ColumnType columnType) {
      if (string.IsNullOrEmpty(columnName)) {
        throw new ArgumentException("Column name cannot be null or empty.");
      }
      if (string.Compare(columnName, "Id", true) == 0) {
        throw new ArgumentException("Column name 'Id' is reserved and cannot be used.");
      }
      if (Columns.ContainsKey(GetColumnID(columnName) ?? -1)) {
        throw new ArgumentException($"Column with name '{columnName}' already exists.");
      }

      var column = new ColumnModel() {
        Id = GetNextNewColumnId(),
        TableId = this.Id,
        Rank = GetNextNewColumnRank(),
        ColumnName = columnName,
        ColumnType = columnType
      };
      this.Columns[column.Id] = column;
      this._columnNameToIdIndex[column.ColumnName] = column.Id;
      foreach (RowModel row in this.Rows.Values) {
        var field = new FieldModel() {
          OwnerRow = row,
          Id = column.Id,
          RowId = row.Id,
          ColumnId = column.Id,
          ValueType = columnType
        };
        row.RowFields[field.Id] = field;
      }
      if (this.Owner != null ) {
        this.Owner.Modified = true;
      }      
      return column;
    }

    public void RemoveColumn(int columnId) {
      if (this.Columns.TryGetValue(columnId, out ColumnModel? column)) {
        foreach (RowModel row in this.Rows.Values) {
          var field = row.RowFields.FirstOrDefault(x => x.Value.ColumnId == column.Id);
          if (field.Key >= 0) {
            row.RowFields.TryRemove( field.Key, out var fieldd);
          }
        }
        _ = this._columnNameToIdIndex.TryRemove(column.ColumnName, out _);
        _ = this._columnOrderByRankToIdIndex.TryRemove(column.Rank, out _);
        this.Columns.Remove(columnId, out var columnM);
        this.RebuildColumnIndex();
        if (this.Owner != null) {
          this.Owner.Modified = true;
        }
      }
    }


    public int? GetColumnID(string columnName) {
      if (Columns == null || Columns.IsEmpty) return null;
      if (string.IsNullOrEmpty(columnName)) return null;
      if (string.Compare(columnName, "Id", true) == 0) {
            return 0; // Id is always column 0
      }
      if (_columnNameToIdIndex.TryGetValue(columnName, out var columnId)) {
            return columnId;
      }      
      return null;
    }    

    public int GetNextNewColumnRank() {      
      if (Columns.Count == 0) return 1;
      int max = Columns.Values.Max(x => x.Rank);
      return (int)(max + 1);      
    }

    public int GetNextNewColumnId() {
      if (Columns.Count == 0) return 1;
      var max = Columns.Keys.Max(x => x);
      return (int)(max + 1);
    }

    [Key(3)]
    public ConcurrentDictionary<int, RowModel> Rows { get; set; } = new();

    [IgnoreMember]
    public ConcurrentDictionary<int, int> RowIndex { get; set; } = new();

    [Key(4)]
    public string OrderByColumnName { get; set; } = "";

    [Key(5)]
    public bool SortAsc { get; set; } = true;

    [IgnoreMember]
    object? IEnumerator.Current => Current;

    [IgnoreMember]
    public RowModel Current { get; private set; } = null!;

    [IgnoreMember]
    public object? this[string columnName] {
      get => Current?[columnName]?.Value;
      set {
        if (Current != null && value != null) {
          Current[columnName].Value = value;
        }
      }
    }

    // Validation events    
    public event EventHandler<RowValidationEventArgs>? RowValidating;
    public event EventHandler<FieldValidationEventArgs>? FieldValidating;
    public event EventHandler<ValidationEventArgs>? ValidationFailed;
    

    // Validation settings
    [IgnoreMember]
    public bool AutoValidate { get; set; } = true;

    [IgnoreMember]
    public bool ValidateOnFocusLost { get; set; } = true;

    [IgnoreMember]
    public bool ValidateOnRowChange { get; set; } = true;

    public ValidationResult ValidateCurrentRow() {

      // No current row is not valid.
      if (Current == null) {
        var aResult = new ValidationResult();
        aResult.AddError("No Current Row.");
        return aResult;
      }

      var result = this.ValidateRow(Current);

      // Fire the row validating event - allows subscribers to cancel or modify
      var rowArgs = new RowValidationEventArgs(Current, result);
      RowValidating?.Invoke(this, rowArgs);

      // Check if validation was cancelled
      if (rowArgs.Cancel) {
        result.AddError("Id", "Validation cancelled by event handler");
      }

      // Fire validation failed event if there are errors
      if (!result.IsValid) {
        ValidationFailed?.Invoke(this, new ValidationEventArgs(result, "Row validation"));
      }

      return result;
    }

    public ValidationResult ValidateField(string columnName) {
      if (Current == null) return new ValidationResult { IsValid = true };

      var result = new ValidationResult { IsValid = true };
      var column = Columns.Values.FirstOrDefault(c => c.ColumnName == columnName);

      if (column != null) {
        // Your existing field validation logic here
        TableValidation.ValidateField(this, Current, column, result);

        // Fire the field validating event
        var fieldArgs = new FieldValidationEventArgs(Current, columnName, result);
        FieldValidating?.Invoke(this, fieldArgs);

        // Check if validation was cancelled
        if (fieldArgs.Cancel) {
          result.AddError("Id", $"{columnName}: Validation cancelled by event handler");
        }

        // Fire validation failed event if there are errors
        if (!result.IsValid) {
          ValidationFailed?.Invoke(this, new ValidationEventArgs(result, $"Field '{columnName}' validation"));
        }
      }

      return result;
    }

    public void RebuildRowIndex() {
      RowIndex.Clear();
      IOrderedEnumerable<RowModel> listRows;      
      if (OrderByColumnName == null || OrderByColumnName == "" || !_columnNameToIdIndex.ContainsKey(OrderByColumnName)) {        
        if (SortAsc) {
          listRows = Rows.Select(x => x.Value).OrderBy(x => x.Id);
        } else {
          listRows = Rows.Select(x => x.Value).OrderByDescending(x => x.Id);
        }        
      } else {                
        if (SortAsc) {
          listRows = Rows.Select(x => x.Value).OrderBy(x => x[OrderByColumnName].Value);
        } else {
          listRows = Rows.Select(x => x.Value).OrderByDescending(x => x[OrderByColumnName].Value);
        }        
      }
      int index = 1;
      foreach (var row in listRows) {
        RowIndex[index] = row.Id;
        index++;        
      }
    }

    public int GetNextNewRowIndex() {
      if (RowIndex.Count == 0) return 1;
      var max = RowIndex.Values.Max(x => x);
      return max + 1;
    }   
    
   
    public bool MoveFirst() {
      if (State != TableState.Browse) {
        Post(); // Auto-post when navigating
      }
      Reset();
      return Current != null;
    }

    public bool MoveLast() {
      if (State != TableState.Browse) {
        Post(); // Auto-post when navigating
      }
      if (RowIndex.Count == 0) return false;
      var lastIndex = RowIndex.Keys.Max();
      if (RowIndex.TryGetValue(lastIndex, out var lastRowId) &&
          Rows.TryGetValue(lastRowId, out var lastRow)) {
        Current = lastRow;
        return true;
      }
      return false;
    }

    public bool MoveTo(int rowNumber) {
      if (State != TableState.Browse) {
        Post(); // Auto-post when navigating
      }
      if (RowIndex.TryGetValue(rowNumber, out var rowId) &&
          Rows.TryGetValue(rowId, out var row)) {
        Current = row;
        return true;
      }
      return false;
    }

    public bool MoveNext() {
      if (State != TableState.Browse) {
        Post(); // Auto-post when navigating
      }
      if (Rows == null || Rows.Count == 0) return false;
      if (Current == null) {
        Current = Rows.Values.FirstOrDefault(RowModel => RowModel.Id >0);
        return Current != null;
      } else {
        int currentIndex = RowIndex.FirstOrDefault(x => x.Value == Current.Id).Key;
        if (currentIndex < 0 || currentIndex >= Rows.Count ) {
          Current = null;
          return false;
        }
        if (RowIndex.TryGetValue(currentIndex + 1, out var nextRowId)) {
          if (Rows.TryGetValue(nextRowId, out var nextRow)) {
            Current = nextRow;
            return true;
          }
        }
        Current = null;
        return false;
      }
    }

    public void Reset() {
      if (State != TableState.Browse) {
        Post(); // Auto-post when resetting
      }
      if (RowIndex.TryGetValue(1, out var firstRowId)) {
        if (Rows.TryGetValue(firstRowId, out var firstRow)) {
          Current = firstRow;
        } else {
          Current = null;
        }
      } else {
        Current = null;
      }      
    }

    public bool FindFirst(string columnName, object value) {
      if (State != TableState.Browse) {
        Post(); // Auto-post when navigating
      }
      if (string.IsNullOrEmpty(columnName) || value == null || Rows == null || Rows.Count == 0) return false;
      if (string.Compare(columnName, "Id", true) == 0) {
        // Special case for Id column
        if (value is int idValue && Rows.TryGetValue(idValue, out var row)) {
          Current = row;
          return true;
        }
        Current = null;
        return false;
      }
      if (_columnNameToIdIndex.TryGetValue(columnName, out var columnId)) {
        string objectValueString = FieldExt.GetValueString(value);
        var firstKvPair = Rows.FirstOrDefault(x => String.Compare( x.Value[columnName].ValueString, objectValueString, true)==0);
        if (firstKvPair.Value != null) {
          Current = firstKvPair.Value;
          return true;
        }        
      }
      Current = null;
      return false;
    }

    private IEnumerable<RowModel> GetOrderedRows() {
      return RowIndex.OrderBy(kvp => kvp.Key)
          .Select(kvp => Rows[kvp.Value]);
    }

    public IEnumerable<RowModel> Where(Func<RowModel, bool> predicate) {
      // Respect the table's current sort order
      return GetOrderedRows().Where(predicate);
    }

    public IEnumerable<T> Select<T>(Func<RowModel, T> selector) {
      return GetOrderedRows().Select(selector);
    }

    public RowModel? FirstOrDefault(Func<RowModel, bool> predicate) {
      return GetOrderedRows().FirstOrDefault(predicate);
    }

    public RowModel AddRow() {
      if (State != TableState.Browse) {
        Post(); // Auto-post any pending changes
      }
      _editingRow = new RowModel(this) {
        Id = GetNextNewRowIndex()
      };

      foreach (var columnId in Columns.Keys) {
        var column = Columns[columnId];
        if (column != null) {
          var field = new FieldModel() {
            OwnerRow = _editingRow,
            Id = columnId,
            RowId = _editingRow.Id,
            ColumnId = columnId,
            ValueType = (ColumnType)column.ColumnType
          };
          _editingRow.RowFields[field.Id] = field;
        }
      }
      Current = _editingRow;
      Rows[_editingRow.Id] = _editingRow;
      var nextRowNumber = GetNextNewRowIndex();
      RowIndex[nextRowNumber] = _editingRow.Id;
      State = TableState.Insert;     
      return _editingRow;
    }
    public void RemoveRow(RowModel row) {
      if (row == null) return;
      if (Rows.ContainsKey(row.Id)) {
        _ = Rows.TryRemove(row.Id, out _);
        RebuildRowIndex();
        if (this.Owner != null) {
          this.Owner.Modified = true;
        }
      }
    }

    public void DeleteCurrentRow() {
      if (Current == null) throw new InvalidOperationException("No current row to delete");
      if (State != TableState.Browse) throw new InvalidOperationException($"Cannot delete while in {State} state");
      RemoveRow(Current);
      Current = null; // Clear current after deletion
      State = TableState.Browse;
    }

    public void Edit() {
      if (Current == null){
        Reset();
        if (Current == null)
          throw new InvalidOperationException("No current row to edit");
      }
      if (State != TableState.Browse) throw new InvalidOperationException($"Cannot edit while in {State} state");

      // Store original values in case of cancel
      _editingRow = new(this);
      foreach (var field in Current.RowFields.Values) {
        _editingRow.RowFields[field.Id] = new FieldModel {
          Id = field.Id,
          RowId = field.RowId,
          ColumnId = field.ColumnId,
          ValueType = field.ValueType,
          ValueString = field.ValueString, // Store original value
          OwnerRow = _editingRow
        };        
      }
      State = TableState.Edit;      
    }

    public void Insert() {
      if (State != TableState.Browse) {
        Post(); // Auto-post any pending changes
      }
      AddRow();      
    }

    public void Post() {
      if (_editingRow == null || Current == null ) {
        throw new InvalidOperationException("No row is being edited or inserted");
      }
      if(State != TableState.Browse) {
        if (AutoValidate) {
          var validation = ValidateCurrentRow();
          if (!validation.IsValid) {
            throw new ValidationException("Cannot post: validation failed");
          }
        }
      }
      
      _editingRow = null;
      State = TableState.Browse;
      if (Owner != null) {
        Owner.Modified = true;
      }
    }

    public void Cancel() {
      if (_editingRow == null || Current == null) {
        throw new InvalidOperationException("No row is being edited or inserted");
      }
      switch (State) {
        case TableState.Edit:
          foreach (var field in _editingRow.RowFields.Values) {
            Current.RowFields[field.Id].Value = field.Value;
          }
          _editingRow = null;
          State = TableState.Browse;
          break;

        case TableState.Insert:
          // Remove the inserting row
          if (_editingRow != null) {
            if (Rows.ContainsKey(_editingRow.Id)) {
              _ = Rows.TryRemove(_editingRow.Id, out _);
              State = TableState.Browse;
              RebuildRowIndex();
              Reset();
            }
            _editingRow = null;            
          }
          break;
      }

      
    }

  }

}
