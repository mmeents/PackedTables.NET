using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Models {
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
