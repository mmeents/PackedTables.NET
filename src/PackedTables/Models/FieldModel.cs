using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackedTables.Extensions;
using PackedTables.Dictionaries;

namespace PackedTables.Models {

  [MessagePackObject]
  public class FieldModel {
    [IgnoreMember]
    public Fields? OwnerFields { get; set; } = null!;

    [Key(0)]
    public Guid Id { get; set; } = Guid.Empty;
    [Key(1)]
    public Guid RowId { get; set; }
    [Key(2)]
    public Guid ColumnId { get; set; }
    [Key(3)]
    public string ValueString { get; set; } = "";

    [IgnoreMember]
    public ColumnType ValueType { get; set; } = ColumnType.Null;

    [IgnoreMember]
    public Object Value {
      get {
        return this.AsObject();
      }
      set {
        if (this.ValueType == FieldExt.GetColumnType(value)) {
          this.ValueString = this.GetColumnValueToString(value);
          if (OwnerFields != null) {
            OwnerFields.NotifyValueChanged(this.RowId);
          }
        } else {
          throw new Exception($"FieldModel:Column ValueType {this.ValueType} does not match new data type {FieldExt.GetColumnType(value)}");
        }

      }
    }

  }
}
