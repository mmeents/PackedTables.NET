using MessagePack;
using PackedTables.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Models {

  [MessagePackObject]
  public class TableModel {

    [Key(0)]
    public Guid Id { get; set; } = Guid.Empty;

    [Key(1)]
    public string Name { get; set; } = "";

    [IgnoreMember]
    public Tables Owner { get; set; } = null!;

    [IgnoreMember]
    public Columns Columns { get; set; } = null!;

    [IgnoreMember] 
    public Fields Fields { get; set; } = null!;

    [IgnoreMember]
    public Rows Rows { get; set; } = null!;

    public Guid GetColumnID(string columnName) {
      if (Columns == null || Columns.Count == 0) return Guid.Empty;
      var column = Columns.FirstOrDefault(x => x.Value.ColumnName == columnName);
      if (column.Key != Guid.Empty) {
        return column.Key;
      }
      return Guid.Empty;
    }

    public IEnumerable<FieldModel> GetFieldsOfRow(Guid rowId) {
      var allFields = Owner.Owner.Fields;
      return allFields.Select( kvp => kvp.Value).Where(x => x.RowId == rowId);
    }

  }
}
