using PackedTables.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Dictionaries {
   

  public class Rows : ConcurrentDictionary<Guid, RowModel> {
    private readonly object _lock = new();
    public TableModel _ownerTable = null;
    public PackedTables? _packedTables = null;  

    public Rows() : base() { 
    }
    public Rows(PackedTables packedTables, IEnumerable<RowModel> rows) : base() {
      _packedTables = packedTables;      
      AsList = rows;
    }   
    public Rows(TableModel owner, IEnumerable<RowModel> rows) : base() {
      _ownerTable = owner;
      if (_ownerTable != null && _ownerTable.Owner?.Owner != null) {
        _packedTables = _ownerTable.Owner.Owner;
      }
      AsList = rows;
    }
    public Rows(TableModel owner) : base() {
      _ownerTable = owner;
      if (_ownerTable != null && _ownerTable?.Owner?.Owner != null) {
        _packedTables = _ownerTable.Owner.Owner;
      }
    }
    public virtual new RowModel this[Guid id] {
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
    
    public RowModel Add(RowModel row) {
      lock (_lock) {
        if (row.Id == Guid.Empty) {
          row.Id = Guid.NewGuid();
        }
        base[row.Id] = row;

        if (_packedTables != null) {
          var table = _packedTables.Package.Tables.FirstOrDefault(x => x.Id == row.TableId);          
          if (table != null) {
            _ownerTable = table;
          }          
        }      
        if (_ownerTable == null) {
          throw new InvalidOperationException("Owner cannot be null when adding a row.");
        }
        row.Owner = _ownerTable;
        var rowfields = new Fields(_ownerTable.GetFieldsOfRow(row.Id), row, _ownerTable.Columns);
        if (row.RowFields == null) {
          row.RowFields = rowfields;
        } else {
          row.RowFields.Synchronize(rowfields);
        }       

        foreach (var columnId in _ownerTable.Columns.Keys) {          
          var column = _ownerTable.Columns[columnId];
          if (column!=null && row[column.ColumnName]==null) {
            var field = new FieldModel() {
              Id = Guid.NewGuid(),
              RowId = row.Id,
              ColumnId = columnId,
              ValueType = (ColumnType)column.ColumnType
            };
            row.RowFields.Add(field);
          }
        }         

        return row;
      }
    }
    public void Remove(Guid id) {
      lock (_lock) {

        if (ContainsKey(id)) {
          _ = base.TryRemove(id, out _);
        }
      }
    }
    public void Remove(RowModel row) {
      lock (_lock) {
        if (row == null) return;
        if (ContainsKey(row.Id)) {
          _ = base.TryRemove(row.Id, out _);
        }
      }
    }
    public IEnumerable<RowModel> AsList {
      get {
        return this.Select(x => x.Value);
      }
      set {
        lock (_lock) {
          if (value == null) return;
          foreach (var item in value) {
            Add(item);
          }
        }
      }
    }

    public IEnumerable<RowModel>? WhereColumnValueEquals(string columnName, object value) {
      if (this == null || this.Count == 0) return new List<RowModel>();
      if (string.IsNullOrEmpty(columnName)) return new List<RowModel>();
      var columnId = _ownerTable.GetColumnID(columnName);
      if (columnId == Guid.Empty) return new List<RowModel>();
      var rows = this.Values.Where(x => x.RowFields != null && x.RowFields[columnId] != null && x.RowFields[columnId].Value.Equals(value));
      return rows;
    }

    public void Synchronize(Rows rows) {
      lock (_lock) {
        if (rows == null || rows.Count == 0) return;
        HashSet<Guid> TableIds = new HashSet<Guid>();
        foreach (var item in rows.Values) {
          TableIds.Add(item.TableId);
          if (ContainsKey(item.Id)) {
            this[item.Id] = item;            
          } else {
            Add(item);
          }
        }

        foreach (var item in this.Values) {
          if (!rows.ContainsKey(item.Id) && TableIds.Contains(item.TableId)) {
            rows.Remove(item);
          }
        }
      }
    }

  }
}
