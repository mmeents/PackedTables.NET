using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Models {

  [MessagePackObject]
  public class DataSetPackage {

    [Key(0)]
    public IEnumerable<ColumnModel> Columns { get; set; } = new List<ColumnModel>();

    [Key(1)]
    public IEnumerable<FieldModel> Fields { get; set; } = new List<FieldModel>();

    [Key(2)]
    public IEnumerable<RowModel> Rows { get; set; } = new List<RowModel>();

    [Key(3)]
    public IEnumerable<TableModel> Tables { get; set; } = new List<TableModel>();


  }
}
