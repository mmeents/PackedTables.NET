using MessagePack;
using System.Collections.Concurrent;

namespace PackedTables.Net
{
  [MessagePackObject]
  public class DataSetModel {

    [Key(0)]
    public ConcurrentDictionary<int, TableModel> Tables { get; set; } = new();

    [Key(1)]
    public ConcurrentDictionary<string, int> NameIndex { get; set; } = new();


  }
}
