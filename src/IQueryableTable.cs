using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Net {
  public interface IQueryableTable {
    IEnumerable<RowModel> Where(Func<RowModel, bool> predicate);
    IEnumerable<T> Select<T>(Func<RowModel, T> selector);
    RowModel? FirstOrDefault(Func<RowModel, bool> predicate);
  }

}
