using PackedTables.Dictionaries;
using PackedTables.Models;

namespace PackedTables
{
    public class PackedTables {
      public PackedTables() {
        Package = new DataSetPackage();
      }
      public PackedTables(DataSetPackage package) {
        Package = package;
      }
      public DataSetPackage Package { get; set; } = new DataSetPackage();

      public Columns Columns {
        get { return new Columns(Package.Columns); }
        set { Package.Columns = value.AsList; }
      }

      public Columns GetColumnsOfTable(Guid tableId) {
        Columns columns = new Columns(Package.Columns.Where(x => x.TableId == tableId));        
        return columns;
      }

      public Fields Fields {
        get { return new Fields(Package.Fields, Columns); }
        set { Package.Fields = value.AsList; }
      }

      public TableModel PopulateTable(TableModel table) {
        table.Columns = GetColumnsOfTable(table.Id);
        var listRows = Package.Rows.Where(x => x.TableId == table.Id);
        List<FieldModel> fields = new List<FieldModel>();
        foreach (var item in listRows) {
          fields.AddRange(Package.Fields.Where(x => x.RowId == item.Id));
        }
        table.Fields = new Fields(fields, table.Columns);
        table.Rows = new Rows(table, listRows);
        return table;
      }

      public Tables Tables {
        get { return new Tables(Package.Tables, this); }
        set { Package.Tables = value.AsList; }
      }

    }
}
