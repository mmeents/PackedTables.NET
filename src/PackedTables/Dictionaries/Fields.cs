using PackedTables.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Dictionaries {
  public class Fields : ConcurrentDictionary<Guid, FieldModel> {
    private readonly Columns _columns;
    private readonly object _lock = new();
    public Fields(Columns columns) : base() {
      _columns = columns;
    }
    public Fields(IEnumerable<FieldModel> fields, Columns columns) : base() {
      _columns = columns;
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

    public FieldModel Add(FieldModel field) {
      lock (_lock) {
        if (field.Id == Guid.Empty) {
          field.Id = GetNextId();
        }
        field.ValueType = (ColumnType)_columns[field.ColumnId].ColumnType;
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

  }
}
