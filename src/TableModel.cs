using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Net
{
  [MessagePackObject(AllowPrivate = true)]
  public class TableModel : IEnumerator<RowModel> {

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
    public void Dispose() {
    }

    [Key(0)]
    public int Id { get; set; } = 0;

    [Key(1)]
    public string Name { get; set; } = "";

    [Key(2)]
    public ConcurrentDictionary<int, ColumnModel> Columns { get; set; } = new();

    [IgnoreMember]
    private ConcurrentDictionary<string, int> _columnNameToIdIndex { get; set; } = new();

    [IgnoreMember]
    private ConcurrentDictionary<int, int> _columnOrderByRankToIdIndex = new();

    public void RebuildColumnIndex() {
      _columnNameToIdIndex.Clear();
      _columnOrderByRankToIdIndex.Clear();
      var listCols = Columns.Select(x => x.Value).OrderBy(x => x.Rank);
      int index = 0;
      foreach (var column in listCols) {
        if (!string.IsNullOrEmpty(column.ColumnName)) {
          _columnNameToIdIndex[column.ColumnName] = column.Id;
        }
        _columnOrderByRankToIdIndex[index] = column.Id;
        index++;
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
    
    public RowModel AddRow() {
      
      RowModel newRow = new RowModel(this){
        Id = GetNextNewRowIndex()
      };      
      Rows[newRow.Id] = newRow; 
      if (RowIndex.Count == 0) {
        RowIndex[1] = newRow.Id; // first row is always at index 1
      } else {
        var nextNewRowNumber = this.RowIndex.Keys.Max() + 1;
        RowIndex[nextNewRowNumber] = newRow.Id;
      }

      foreach (var columnId in Columns.Keys) {
        var column = Columns[columnId];
        if (column != null) {
          var field = new FieldModel() {           
            OwnerRow = newRow,
            Id = columnId,
            RowId = newRow.Id,
            ColumnId = columnId,
            ValueType = (ColumnType)column.ColumnType
          };
          newRow.RowFields[field.Id] = field;
        }
      }
      Current = newRow;
      if (this.Owner != null) {
        this.Owner.Modified = true;
      }
      return newRow;      
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


    [IgnoreMember]
    object? IEnumerator.Current => Current;

    [IgnoreMember]
    public RowModel? Current { get; private set; } = null;
      

    public bool MoveNext() {
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
      if (string.IsNullOrEmpty(columnName) || value == null || Rows == null || Rows.Count == 0) return false;
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

  }
}
