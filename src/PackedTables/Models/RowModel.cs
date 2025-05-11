using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackedTables.Dictionaries;

namespace PackedTables.Models {

  [MessagePackObject]
  public class RowModel {
    public RowModel() { }
    public RowModel(TableModel owner) {
      Owner = owner;      
    }

    [Key(0)]
    public Guid Id { get; set; } = Guid.Empty;

    [Key(1)]
    public Guid TableId { get; set; } = Guid.Empty;

    [IgnoreMember]
    public TableModel? Owner { get; set; } = null;

    [IgnoreMember]
    public Fields? RowFields { get; set; }

    [IgnoreMember]
    public FieldModel? this[string columnName] {
      get {
        if (Owner == null || columnName == null || columnName.Length == 0) return null;
        var columnId = Owner!.GetColumnID(columnName);
        FieldModel? field = RowFields?.Values.FirstOrDefault(x => x.ColumnId == columnId);
        return field;
      }
      set {
        if (Owner == null || columnName == null || columnName.Length == 0) return;
        var columnId = Owner!.GetColumnID(columnName);
        if (columnId == Guid.Empty) throw new Exception("Column not found");
        if (value != null) {          
          if (value.ColumnId != columnId) throw new Exception("ColumnId mismatch");
          RowFields?.Add(value);
        } else {
          var field = RowFields?.Values.FirstOrDefault(x => x.ColumnId == columnId);
          if (field != null) {
            RowFields!.Remove(field);
          }
        }
      }
    }

  }
}
