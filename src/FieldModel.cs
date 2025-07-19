using MessagePack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Net {
  [MessagePackObject(AllowPrivate = true)]
  public class FieldModel {

    [IgnoreMember]
    public RowModel? OwnerRow { get; set; } = null;

    [Key(0)]
    public int Id { get; set; } = 0;
    [Key(1)]
    public int RowId { get; set; }
    [Key(2)]
    public int ColumnId { get; set; }
    [Key(3)]
    public string ValueString { get; set; } = "";
    [Key(4)]
    public ColumnType ValueType { get; set; } = ColumnType.Null;

    [IgnoreMember] 
    private object? _nativeValue = null;

    [IgnoreMember]
    public Object Value {
      get {
        if (_nativeValue == null) {
          _nativeValue = this.AsObject();
        }
        return _nativeValue;
      }
      set {
        if (this.ValueType == FieldExt.GetColumnType(value)) {
          _nativeValue = value;
          this.ValueString = this.GetColumnValueToString(value);
        } else {
          if (this.ValueType == ColumnType.Int32) { 
            if (int.TryParse( Value.AsString(), out int intValue)) {
              _nativeValue = intValue;
              this.ValueString = FieldExt.GetValueString(intValue);
              return;
            } 
          }
          throw new ArgumentException($"FieldModel:Column ValueType {this.ValueType} does not match new data type {FieldExt.GetColumnType(value)}");
        }      
        if (this.OwnerRow != null && this.OwnerRow.Owner != null && this.OwnerRow.Owner.Owner != null) {
          this.OwnerRow.Owner.Owner.Modified = true;
        }
      }
    }
  }


  public static class FieldExt {

    public static Object AsObject(this FieldModel field) {
      Object ret = "";
      switch (field.ValueType) {
        case ColumnType.Null: ret = ""; break;
        case ColumnType.String: ret = field.ValueString; break;
        case ColumnType.Int32: ret = field.ValueString.AsInt32(); break;
        case ColumnType.DateTime: ret = field.ValueString.AsDateTime(); break;
        case ColumnType.Boolean: ret = field.ValueString.AsBoolean(); break;
        case ColumnType.Decimal: ret = field.ValueString.AsDecimal(); break;
        case ColumnType.Bytes: ret = field.ValueString.FromStringAsBytes(); break;
        case ColumnType.Int64: ret = field.ValueString.AsInt64(); break;
        case ColumnType.Guid: ret = Guid.Parse(field.ValueString); break;
        case ColumnType.Unknown: ret = ""; break;
      }
      return ret;
    }

    public static string GetValueString(Object value) {
      if (value == null) {
        return "";
      } else if (value is string valString) {
        return valString;
      } else if (value is int valInt) {
        return valInt.ToString();
      } else if (value is long valLong) {
        return valLong.ToString();
      } else if (value is DateTime valDateTime) {
        return valDateTime.AsStrDateTime24H();
      } else if (value is bool valBool) {
        return valBool.ToString();
      } else if (value is Decimal valDecimal) {
        return valDecimal.ToString();
      } else if (value is byte[] valBytes) {
        return valBytes.FromBytesAsString();
      } else if (value is Guid valGuid) {
        return valGuid.ToString();
      } else {
        throw new Exception($"Unknow Object ValueType");
      }      
    }

    public static string GetColumnValueToString(this FieldModel field, Object value) {

      if (value == null) {
        field.ValueString = "";
      } else if (value is string valString) {
        field.ValueString = valString;
      } else if (value is int valInt) {
        field.ValueString = valInt.ToString();
      } else if (value is long valLong) {
        field.ValueString = valLong.ToString();
      } else if (value is DateTime valDateTime) {
        field.ValueString = valDateTime.AsStrDateTime24H();
      } else if (value is bool valBool) {
        field.ValueString = valBool.ToString();
      } else if (value is Decimal valDecimal) {
        field.ValueString = valDecimal.ToString();
      } else if (value is byte[] valBytes) {
        field.ValueString = valBytes.FromBytesAsString();
      } else if (value is Guid valGuid) {
        field.ValueString = valGuid.ToString();
      } else {
        throw new Exception($"FieldModel:Column ValueType {field.ValueType} does not match new data type {FieldExt.GetColumnType(value)}");
      }
      return field.ValueString;
    }

    public static ColumnType GetColumnType(Object value) {
      if (value == null) {
        return ColumnType.Null;
      } else if (value is string) {
        return ColumnType.String;
      } else if (value is int) {
        return ColumnType.Int32;
      } else if (value is long) {
        return ColumnType.Int64;
      } else if (value is DateTime) {
        return ColumnType.DateTime;
      } else if (value is bool) {
        return ColumnType.Boolean;
      } else if (value is Decimal) {
        return ColumnType.Decimal;
      } else if (value is byte[]) {
        return ColumnType.Bytes;
      } else if (value is Guid) {
        return ColumnType.Guid;
      }
      return ColumnType.Unknown;
    }  
  }
}
