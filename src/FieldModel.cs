using MessagePack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Net {
  [MessagePackObject]
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
    public Object Value {
      get {
        return this.AsObject();
      }
      set {
        if (this.ValueType == FieldExt.GetColumnType(value)) {
          this.ValueString = this.GetColumnValueToString(value);
          //       if (OwnerFields != null) {
          //         OwnerFields.NotifyValueChanged(this.RowId);
          //       }
        } else {
          throw new Exception($"FieldModel:Column ValueType {this.ValueType} does not match new data type {FieldExt.GetColumnType(value)}");
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
        case ColumnType.Bytes: ret = field.ValueString.FromUTF8AsBytes(); break;
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
        return valBytes.FromUTF8BytesAsString();
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
        field.ValueString = valBytes.FromUTF8BytesAsString();
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

    public static int AsInt32(this FieldModel field) {
      return field.ValueString.AsInt32();
    }
    public static long AsInt64(this FieldModel field) {
      return field.ValueString.AsInt64();
    }
    public static byte[] AsBytes(this FieldModel field) {
      return field.ValueString.FromUTF8AsBytes();
    }
    public static bool AsBoolean(this FieldModel field) {
      return field.ValueString.AsBoolean();
    }
    public static DateTime AsDateTime(this FieldModel field) {
      return field.ValueString.AsDateTime();
    }
    public static Decimal AsDecimal(this FieldModel field) {
      return field.ValueString.AsDecimal();
    }

    public static int AsInt32(this string value) {
      return int.Parse(value);
    }
    public static long AsInt64(this string value) {
      return long.Parse(value);
    }
    public static byte[] AsBase64Bytes(this string value) {
      return Convert.FromBase64String(value);
    }
    public static bool AsBoolean(this string value) {
      return Convert.ToBoolean(value);
    }
    public static DateTime AsDateTime(this string value) {
      return DateTime.Parse(value);
    }
    public static Decimal AsDecimal(this string value) {
      return Decimal.Parse(value);
    }
    public static byte[] FromUTF8AsBytes(this string text) {
      return Encoding.UTF8.GetBytes(text);
    }
    public static string FromUTF8BytesAsString(this byte[] bytes) {
      return Encoding.UTF8.GetString(bytes);
    }
    public static string AsString(this int value) {
      return value.ToString();
    }


    public static int AsInt32(this Object value) {
      return Convert.ToInt32(value);
    }
    public static long AsInt64(this Object value) {
      return Convert.ToInt64(value);
    }
    public static byte[] AsBytes(this Object value) {
      return (byte[])value;
    }
    public static bool AsBoolean(this Object value) {
      return Convert.ToBoolean(value);
    }
    public static DateTime AsDateTime(this Object value) {
      return Convert.ToDateTime(value);
    }
    public static Decimal AsDecimal(this Object value) {
      return Convert.ToDecimal(value);
    }
    public static string AsString(this Object value) {
      return value?.ToString() ?? "";
    }


    #region Date to string 
    /// <summary> Day to string Sortable yyyy-MM-dd</summary>
    /// <returns> string </returns>
    public static string AsStrDate(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd}", x);
    }
    /// <summary> DateTime to string yyyy-MM-dd hh:mm:ss.FFF tt </summary>
    /// <returns> string </returns>
    public static string AsStrDateTime12H(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd hh:mm:ss.FFF tt}", x);
    }
    /// <summary> DateTime to string yyyy-MM-dd HH:mm:ss.FFF</summary>
    /// <returns> string </returns>
    public static string AsStrDateTime24H(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd HH:mm:ss.FFF}", x);
    }
    /// <summary> DateTime to string DateTime h:mm:ss tt</summary>
    /// <returns> string </returns>
    public static string AsStrTime(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:h:mm:ss tt}", x);
    }
    /// <summary> DateTime to string Day Time MM/dd/yyyy hh:mm</summary>
    /// <returns> string </returns>
    public static string AsStrDayHHMM(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:MM/dd/yyyy hh:mm}", x);
    }
    /// <summary> DateTime to string Day MM/dd/yyyy</summary>
    /// <returns> string </returns>
    public static string AsStrDay(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:MM/dd/yyyy}", x);
    }

    #endregion


    public static string[] Parse(this string content, string delims) {
      return content.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    }
    public static string ParseFirst(this string content, string delims) {
      string[] sr = content.Parse(delims);
      if (sr.Length > 0) {
        return sr[0];
      }
      return "";
    }
    public static string ParseLast(this string content, string delims) {
      string[] sr = content.Parse(delims);
      if (sr.Length > 0) {
        return sr[^1];
      }
      return "";
    }

    /// <summary>
    /// async read file from file system into string
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static async Task<string> ReadAllTextAsync(this string filePath) {
      using var streamReader = new StreamReader(filePath);
      return await streamReader.ReadToEndAsync();
    }

    /// <summary>
    /// async write content to fileName location on file system. 
    /// </summary>
    /// <param name="Content"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static async Task<int> WriteAllTextAsync(this string Content, string fileName) {
      using var streamWriter = new StreamWriter(fileName);
      await streamWriter.WriteAsync(Content);
      return 1;
    }
  }


}
