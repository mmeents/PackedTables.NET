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
    [IgnoreMember]
    private TableModel? _owner = null;

    [Key(0)]
    public int Id { get; set; } = 0;

    [Key(1)]
    public int TableId { get; set; } = 0;

    [Key(2)]
    public ConcurrentDictionary<int, FieldModel> RowFields { get; set; } = new();

    [IgnoreMember]
    private FieldModel? _rowId = null;

    [IgnoreMember]
    public FieldModel RowId { 
      get{
        if (_rowId == null) {
          _rowId = new FieldModel() {
            Id = 0,
            RowId = this.Id,
            ColumnId = 0, // This is a special case, RowId does not have a column
            OwnerRow = this,
            ValueType = ColumnType.Int32,
            ValueString = this.Id.AsString()
          };          
        }
        return _rowId;
      } 
    }

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
        if (Owner == null || string.IsNullOrEmpty(columnName)) throw new ArgumentException($"Owner is null or bad column {columnName}");
        if (string.Compare(columnName, "Id", true)==0) {
          return RowId;
        }
        var columnId = Owner!.GetColumnID(columnName) ?? throw new ArgumentException($"Get Column not found {columnName}");         
        var fieldExists = RowFields.Any(x => x.Value.ColumnId == columnId);
        if (!fieldExists) throw new ArgumentException("Rowfield not found");
        var field = RowFields.First(x => x.Value.ColumnId == columnId);
        return field.Value;
      }
      set {
        if (Owner == null || columnName == null || columnName.Length == 0) throw new ArgumentException("Owner is null or bad column name ");
        var columnId = Owner!.GetColumnID(columnName) ?? throw new ArgumentException($"Set column {columnName} not found");
        if (value != null) {
          if (value.ColumnId != columnId) throw new ArgumentException("ColumnId mismatch");
          RowFields[value.Id] = value;
          if (this.Owner != null && this.Owner.Owner != null) {
            this.Owner.Owner!.Modified = true;
          }
        } else {
          var field = RowFields?.FirstOrDefault(x => x.Value.ColumnId == columnId);
          if (field != null) {
            RowFields!.Remove(field.Value.Value.Id, out var field1);
            if (this.Owner != null && this.Owner.Owner != null) {
              this.Owner.Owner!.Modified = true;
            }
          }
        }
      }
    }
  }
}
