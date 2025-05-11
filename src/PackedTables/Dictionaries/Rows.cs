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
    private readonly TableModel _owner;
    public Rows(TableModel owner) : base() {
      _owner = owner;
    }
    public Rows(TableModel owner, IEnumerable<RowModel> rows) : base() {
      _owner = owner;
      AsList = rows;
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
        row.Owner = _owner;
        row.RowFields = new Fields(_owner.GetFieldsOfRow(row.Id), _owner.Columns);
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

  }
}
