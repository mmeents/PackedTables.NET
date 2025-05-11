using PackedTables.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Dictionaries {
  public class Tables : ConcurrentDictionary<Guid, TableModel> {
    public PackedTables Owner { get; set;}
    private readonly object _lock = new();

    public Tables(PackedTables owner) : base() {
      Owner = owner;
    }
    public Tables(IEnumerable<TableModel> tables, PackedTables owner) : base() {
      Owner = owner;
      AsList = tables;
    }

    public Tables(IEnumerable<TableModel> tables) : base() {
      AsList = tables;
    }

    public virtual new TableModel this[Guid id] {
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

    public TableModel Add(TableModel table) {
      lock (_lock) {
        if (table.Id == Guid.Empty) {
          table.Id = Guid.NewGuid();
        }
        base[table.Id] = table;
        table.Owner = this;
        Owner.PopulateTable(table);
        return table;
      }
    }

    public void Remove(Guid id) {
      lock (_lock) {
        if (ContainsKey(id)) {
          _ = base.TryRemove(id, out _);
        }
      }
    }

    public void Remove(TableModel table) {
      lock (_lock) {
        if (ContainsKey(table.Id)) {
          _ = base.TryRemove(table.Id, out _);
        }
      }
    }

    public IEnumerable<TableModel> AsList {
      get {
        return this.Values.ToList();
      }
      set {
        lock (_lock) {
          if (value != null) {
            foreach (var item in value) {
              Add(item);
            }
          }
        }        
      }
    }

  }
}
