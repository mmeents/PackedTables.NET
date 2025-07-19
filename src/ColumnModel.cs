using MessagePack;

namespace PackedTables.Net
{
  [MessagePackObject(AllowPrivate = true)]
  public partial class ColumnModel {

    [Key(0)]
    public int Id { get; set; } = 0;

    [Key(1)]
    public int TableId { get; set; } = 0;

    [Key(2)]
    public int Rank { get; set; } = 0;

    [Key(3)]
    private int _columnType = 0;

    [IgnoreMember]
    public ColumnType ColumnType {
      get { return (ColumnType)_columnType; }
      set { _columnType = (int)value; }
    }

    [Key(4)]
    public string ColumnName { get; set; } = "";

    [Key(5)] public bool IsRequired { get; set; } = false;
    [Key(6)] public bool IsVisible { get; set; } = true;
    [Key(7)] public int MaxLength { get; set; } = 0; // 0 = no limit
    [Key(8)] public string DefaultValue { get; set; } = "";
    [Key(9)] public string ValidationRegex { get; set; } = "";
    [Key(10)] public string DisplayName { get; set; } = "";
    [Key(11)] public string Description { get; set; } = "";

  }

  public enum ColumnType {
    Unknown = 0,
    Null = 1,
    Boolean = 2,
    Int32 = 3,
    Int64 = 4,
    Decimal = 5,
    DateTime = 6,
    Guid = 7,
    String = 8,
    Bytes = 9
  }

}
