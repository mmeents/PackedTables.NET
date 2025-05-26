using PackedTables.Dictionaries;
using PackedTables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Extensions {
  public static class RowsExt {
    public static RowModel? ColumnValueEquals(this Rows rows, string columnName, object value) {
      if (rows == null || rows.Count == 0) return null;
      var result = rows.WhereColumnValueEquals(columnName, value);      
      return result?.FirstOrDefault();
    }

  }
}
