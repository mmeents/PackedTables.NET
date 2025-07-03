using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Net {

  [MessagePackObject]
  public class RowModel {

    private TableModel? _owner = null;

    [Key(0)]
    public int Id { get; set; } = 0;

    [Key(1)]
    public int TableId { get; set; } = 0;

    [Key(2)]
    public ConcurrentDictionary<int, FieldModel> RowFields { get; set; } = new();

    public RowModel() {
      Owner = null;
    }

    public RowModel(TableModel owner) {
      Owner = owner;
    }


    [IgnoreMember]
    public TableModel? Owner {
      get { return _owner; }
      set {
        _owner = value;
        if (_owner != null && _owner.Id != TableId) {
          TableId = _owner.Id;
        }
      }
    }

    [IgnoreMember]
    public FieldModel this[string columnName] {
      get {
        if (Owner == null || string.IsNullOrEmpty(columnName)) throw new Exception("Owner is null or bad column name");
        var columnId = Owner!.GetColumnID(columnName) ?? throw new Exception("Column not found");         
        var fieldExists = RowFields.Any(x => x.Value.ColumnId == columnId);
        if (!fieldExists) throw new Exception("Rowfield not found");
        var field = RowFields.First(x => x.Value.ColumnId == columnId);
        return field.Value;
      }
      set {
        if (Owner == null || columnName == null || columnName.Length == 0) throw new Exception("Owner is null or bad column name ");
        var columnId = Owner!.GetColumnID(columnName) ?? throw new Exception("Column not found");
        if (value != null) {
          if (value.ColumnId != columnId) throw new Exception("ColumnId mismatch");
          RowFields[value.Id] = value;
          this.Owner.Owner!.Modified = true;
        } else {
          var field = RowFields?.FirstOrDefault(x => x.Value.ColumnId == columnId);
          if (field != null) {
            RowFields!.Remove(field.Value.Value.Id, out var field1);
            this.Owner.Owner!.Modified = true;
          }
        }
      }
    }
  }
}
