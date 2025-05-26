using PackedTables.Dictionaries;
using PackedTables.Models;
using PackedTables.Extensions;
using MessagePack;

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

    public Tables Tables {
      get { return new Tables(Package.Tables, this); }
      set { Package.Tables = value.AsList; }
    }

    public Columns Columns {
      get { return new Columns(Package.Columns); }
      set { Package.Columns = value.AsList; }
    }

    public Fields Fields {
      get { return new Fields(Package.Fields, null, Columns); }
      set { Package.Fields = value.AsList; }
    }

    public Rows Rows {
      get { return new Rows(this, Package.Rows); }
      set { Package.Rows = value.AsList; }
    }

    public Columns GetColumnsOfTable(Guid tableId) {
      Columns columns = new Columns(Package.Columns.Where(x => x.TableId == tableId));
      return columns;
    }

    /// <summary>
    /// Populates the table with its columns and rows.  MessagePack will not serialize the rows and columns, so we need to do it manually.
    /// idea is after deserialization, we are left with 4 dictionaries: Tables, Columns, Fields, Rows.  We can then populate the TableModel with the data from these dictionaries.
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public TableModel PopulateTable(TableModel table) {
      var cols = GetColumnsOfTable(table.Id);
      cols.Synchronize(table.Columns); // Synchronize the columns with the table model columns.
      table.Columns.Synchronize(cols); // Synchronize the columns with the table model columns.
      var listRows = Package.Rows.Where(x => x.TableId == table.Id);
      List<FieldModel> fields = new List<FieldModel>();      
      foreach (var item in listRows) {
        if (table.Rows.ContainsKey(item.Id)) {
          table.Rows[item.Id].RowFields = new Fields(Package.Fields.Where(x => x.RowId == item.Id), table.Rows[item.Id], table.Columns);
        } else {
          table.Rows[item.Id] = item;
        }                
        fields.AddRange(Package.Fields.Where(x => x.RowId == item.Id));
      }
      if (table.Fields != null) {
        fields.AddRange( table.Fields.AsList.ToList()); // Get the fields from the table model, this is a list of fields.      
      }
      table.Fields = new Fields(fields, null, table.Columns);
      table.Rows = new Rows(table, listRows);
      return table;
    }

    public Fields GetFieldsOfTable(Guid tableId) {
      var listRows = Package.Rows.Where(x => x.TableId == tableId);
      var listFields = Package.Fields.Where(x => listRows.Select(r => r.Id).Contains(x.RowId));
      return new Fields(listFields, null, Columns);
    }

    public Rows GetRowsOfTable(Guid tableId) {
      var listRows = Package.Rows.Where(x => x.TableId == tableId);
      return new Rows(Tables[tableId], listRows);
    }

    public int TableCount() {
      var tables = Tables;
      return tables.Keys.Count;
    }

    public TableModel AddTable(string tableName) {
      var table = new TableModel() {
        Id = Guid.NewGuid(),
        Name = tableName
      };
      return Tables.Add(table);
    }

    /// <summary>
    /// Gets or sets a TableModel by its name.  If the table does not exist, it will return null.
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public TableModel? this[string tableName] {  
      get {
        var table = Tables[tableName]; 
        if (table == null) return null; // If the table does not exist, return null.
        SynchronizeTable(table); // Ensure the table is synchronize with the package 
        return table;
      }      
      set {           
        var table = Tables[tableName];
        if (table != null) {
          if (value == null) {
            Tables.Remove(table.Id);
          } else {
            SynchronizeTable(table); // Update the existing table with the new values
          }          
        } else {
          if (value != null) {
            SynchronizeTable(value); // If the table does not exist, add it to the package
          }
        }
      }
    }

    /// <summary>
    /// Inverse of the PopulateTable method.  This method updates the package with the changes made to the table model.
    /// </summary>
    /// <param name="saveTableModel">TableModel from Tables dictionary</param>
    public void SynchronizeTable(TableModel saveTableModel) {
      var tableId = saveTableModel.Id;  // Get the table ID from the saveTableModel
            
      var newColumns = Columns;         // Get the current columns from the package, this transorms the columns list into a dictionary.
      var newFields = Fields;           // Get the current fields from the package, this transorms the fields list into a dictionary.
      var newRowsList = Rows;   // Get the current rows from the package, this is not a transformation.
      var newTables = Tables;           // Get the current tables from the package 

      newTables[saveTableModel.Id] = saveTableModel;

      newColumns.Synchronize(saveTableModel.Columns); // Synchronize the columns with the saveTableModel columns.
      newFields.Synchronize(saveTableModel.Fields); // Synchronize the fields with the saveTableModel fields.
      newRowsList.Synchronize(saveTableModel.Rows); // Synchronize the rows with the saveTableModel rows.

      Columns = newColumns;             // Update the package with the new columns. 
      Fields = newFields;               // Update the package with the new fields.
      Rows = newRowsList;               // Update the package with the new rows.
      Tables = newTables;               // Update the package with the new tables.
    }



  }
}

