using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace PackedTables.Models {

  [MessagePackObject]
  public class ColumnModel {

    [Key(0)]
    public Guid Id { get; set; } = Guid.Empty;

    [Key(1)]
    public Guid TableId { get; set; } = Guid.Empty;

    [Key(2)]
    public int Rank { get; set; } = 0;

    [Key(3)]
    public short ColumnType { get; set; } = 0;

    [Key(4)]
    public string ColumnName { get; set; } = "";

  }


}
