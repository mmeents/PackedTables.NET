using PackedTables.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Dictionaries {
  public class Columns : ConcurrentDictionary<Guid, ColumnModel> {
    private readonly object _lock = new object();
    private readonly ConcurrentDictionary<string, ColumnModel> byName;
    public Columns() : base() {
      byName = new ConcurrentDictionary<string, ColumnModel>();
    }
    public Columns(IEnumerable<ColumnModel> columns) : base() {
      byName = new ConcurrentDictionary<string, ColumnModel>();
      AsList = columns;
    }
    public virtual Boolean Contains(Guid index) {
      lock (_lock) {
        try {
          return base.ContainsKey(index);
        } catch {
          return false;
        }
      }
    }
    public virtual Boolean Contains(string ColumnName) {
      lock (_lock) {
        try {
          return byName.ContainsKey(ColumnName);
        } catch {
          return false;
        }
      }
    }
    public virtual new ColumnModel? this[Guid id] {
      get {
        lock (_lock) {
          return Contains(id) ? (ColumnModel)base[id] : null;
        }
      }
      set {
        var lid = id;
        lock (_lock) {
          if (value != null) {
            if (id == Guid.Empty) {
              value.Id = Guid.NewGuid();              
              value.Rank = GetNextRank();
              lid = value.Id;
            }
            if (lid != value.Id) {
              value.Id = lid;
            }
            base[lid] = value;
            byName[value.ColumnName] = value;
          } else {
            var valName = base[id]?.ColumnName ?? "";
            if ((valName != "") && Contains(valName)) {
              _ = byName.TryRemove(valName, out _);
            }
            if (Contains(id)) {
              _ = base.TryRemove(id, out _);
            }
          }
        }
      }
    }

    public ColumnModel? ByName(string Name) {
      if (Name == null) return null;
      if (Contains(Name)) {
        return byName[Name];
      }
      return null;
    }

    public virtual void Remove(Guid id) {
      if (Contains(id)) {
        var valName = base[id]?.ColumnName ?? "";
        if ((valName != "") && Contains(valName)) {
          _ = byName.TryRemove(valName, out _);
        }
        _ = base.TryRemove(id, out _);
      }
    }

    public Guid GetNextId() {            
      return Guid.NewGuid();
    }

    public int GetNextRank() {
      lock (_lock) {
        if (base.Count == 0) return 0;
        var max = base.Values.Max(x => x.Rank);
        return max + 1;
      }
    }

    public ColumnModel Add(ColumnModel column) {
      lock (_lock) {
        if (column.Id == Guid.Empty) {
          column.Id = Guid.NewGuid();          
          column.Rank = column.Rank==0 ? GetNextRank() : column.Rank;
        }
        base[column.Id] = column;
        byName[column.ColumnName] = column;
        return column;
      }
    }

    public IEnumerable<ColumnModel> AsList {
      get {
        lock (_lock) {
          return base.Values.ToList().OrderBy(x => x.Rank);
        }
      }
      set {
        base.Clear();
        byName.Clear();
        foreach (var x in value) {
          Add(x);
        }
      }
    }
  }

}
