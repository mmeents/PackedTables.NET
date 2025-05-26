using PackedTables.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Dictionaries {

  /// <summary>
  ///  Every row has a collection of fields, which are governed by the columns in table.  
  ///  Columns is a pointer to the tables-columns list.  it define the column name and type of data that can be stored in each field of a row.  
  /// </summary>
  public class Fields : ConcurrentDictionary<Guid, FieldModel> {
    private RowModel? _ownerRow = null;
    public Columns _columns = new();
    private readonly object _lock = new();
    public Fields(Columns columns) : base() {
      _columns = columns;
    }
    public Fields(IEnumerable<FieldModel> fields, RowModel ownerRow, Columns columns) : base() {
      _ownerRow = ownerRow;
      _columns = columns;
      if (fields == null) return;
      AsList = fields;
    }

    public virtual new FieldModel this[Guid id] {
      get {
        return base[id];
      }
      set {
        lock (_lock) {
          if (value != null) {
            Add(value);
          } else {
            Remove(id);
          }
        }
      }
    }

    public Guid GetNextId() {      
      return Guid.NewGuid();
    }

    public void NotifyValueChanged(Guid fieldId) {
      if (_ownerRow == null) return;
      if (_ownerRow.Owner == null) return;
      _ownerRow.Owner.NotifyValueChanged(_ownerRow.Id);
    }

    public FieldModel Add(FieldModel field) {
      lock (_lock) {
        if (field.Id == Guid.Empty) {
          field.Id = GetNextId();
        }
        if (field.OwnerFields == null) {
          field.OwnerFields = this;
        }
        var fieldColumnId = field.ColumnId;
        if (_columns.TryGetValue(fieldColumnId, out var column)) {
            field.ValueType = (ColumnType)column.ColumnType;
        } else {
            throw new KeyNotFoundException($"The column with ID {fieldColumnId} was not found in _columns.");
        }
        base[field.Id] = field;
        return field;
      }
    }

    public void Remove(Guid id) {
      lock (_lock) {
        if (ContainsKey(id)) {
          _ = base.TryRemove(id, out _);
        }
      }
    }

    public void Remove(FieldModel field) {
      lock (_lock) {
        if (field == null) return;
        if (ContainsKey(field.Id)) {
          _ = base.TryRemove(field.Id, out _);
        }
      }
    }

    public IEnumerable<FieldModel> AsList {
      get {
        lock (_lock) {
          return [.. base.Values];
        }
      }
      set {
        lock (_lock) {
          base.Clear();
          foreach (var item in value) {
            Add(item);
          }
        }
      }
    }

    public void Synchronize(Fields fields) {
      lock (_lock) {     
        HashSet<Guid> rowIds = new HashSet<Guid>();       
        HashSet<Guid> columnIds = new HashSet<Guid>();
        foreach (var item in fields.Values) {
          rowIds.Add(item.Id);
          columnIds.Add(item.ColumnId);
          if (base.ContainsKey(item.Id)) {
            var field = base[item.Id];
            field.RowId = item.RowId;
            field.ColumnId = item.ColumnId;
            field.ValueType = item.ValueType;
            field.Value = item.Value;
            var fieldColumnId = field.ColumnId;
            if (_columns.TryGetValue(fieldColumnId, out var column)) {
              field.ValueType = (ColumnType)column.ColumnType;
            } else {
              throw new KeyNotFoundException($"The column with ID {fieldColumnId} was not found in _columns.");
            }
          } else {
            Add(item);
          }
        }
        
        var list = base.Values.Where(f => rowIds.Contains(f.RowId) && !columnIds.Contains(f.ColumnId)).ToList();
        foreach (var item in list) {
          if (!fields.ContainsKey(item.Id)) {
            Remove(item.Id);
          }
        }
      }
    }

    public void ClearAll() {
      lock (_lock) {
        base.Clear();
      }
    }

    public new void Clear() {
      ClearAll();
    }

  }
}
