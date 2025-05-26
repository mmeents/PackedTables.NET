using PackedTables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Extensions {
  public static class PackedTablesExt {
    public static PackedTables LoadFromFile(this PackedTables packedTables, string fileName) {
      if (File.Exists(fileName)) {
        var encoded = Task.Run(async () => await fileName.ReadAllTextAsync().ConfigureAwait(false)).GetAwaiter().GetResult();
        packedTables.LoadFromBase64String(encoded);
      }
      return packedTables;
    }

    public static void SaveToFile(this PackedTables packedTables, string fileName) {
      var base64 = packedTables.SaveToBase64String();
      Task.Run(async () => await base64.WriteAllTextAsync(fileName).ConfigureAwait(false)).GetAwaiter().GetResult();
    }

    public static string SaveToBase64String(this PackedTables packedTables) {
      var encoded = MessagePack.MessagePackSerializer.Serialize(packedTables.Package);
      var base64 = Convert.ToBase64String(encoded);
      return base64;
    }

    public static PackedTables LoadFromBase64String(this PackedTables packedTables, string base64) {
      if (base64 == null || base64.Length == 0) {
        packedTables.Package = new DataSetPackage();
      } else {
        var decoded = Convert.FromBase64String(base64);
        packedTables.Package = MessagePack.MessagePackSerializer.Deserialize<DataSetPackage>(decoded);
      }
      return packedTables;
    }

    public static string SaveToJson(this PackedTables packedTables) {
      return MessagePack.MessagePackSerializer.SerializeToJson(packedTables.Package);
    }

    public static PackedTables LoadFromJson(this PackedTables packedTables, string json) {
      if (json == null || json.Length == 0) {
        packedTables.Package = new DataSetPackage();
      } else {
        var byteArray = MessagePack.MessagePackSerializer.ConvertFromJson(json);

        packedTables.Package = MessagePack.MessagePackSerializer.Deserialize<DataSetPackage>(byteArray);
      }
      return packedTables;
    }
    public static TableModel? GetTableByName(this PackedTables packedTables, string tableName) {
      var tables = packedTables.Tables;
      var table = tables.FirstOrDefault(t => t.Value.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
      if (table.Key != Guid.Empty) {
        return table.Value;
      } else {
        return null;
      }
    }

    public static PackedTables SaveTableToPackage(this PackedTables packedTables, TableModel tableModel) {
      var fields = packedTables.Fields;
      var columns = packedTables.Columns;      
      var tables = packedTables.Package.Tables.ToList();
      var rows = packedTables.Rows;

      var tblModelId = tableModel.Id;
      var tbl = tables.FirstOrDefault(x => x.Id == tblModelId);
      if (tbl == null) {
        tables.Add(tableModel);
      } else {      
        tbl.Name = tableModel.Name; // Update the name of the table if it exists.        
      }
      packedTables.Package.Tables = tables; // Update the package with the new list of tables.
      columns.Syncronize(tableModel.Columns); // Syncronize the columns with the table model columns.      
      foreach (var row in tableModel.Rows.AsList) {
        tableModel.Fields.Syncronize(row.RowFields);         
      }
      fields._columns = columns;
      fields.Syncronize(tableModel.Fields); // Syncronize the fields with the table model fields.
      rows._ownerTable = tableModel; // Set the owner of the rows to the table model.
      rows._packedTables = packedTables; // Set the packed tables of the rows to the packed tables.
      rows.Syncronize(tableModel.Rows); // Syncronize the rows with the table model rows.
      

      packedTables.Package.Rows = tableModel.Rows.AsList.ToList();

      packedTables.Fields = fields;
      packedTables.Columns = columns;
      packedTables.Rows = rows;
      return packedTables;
    }

  }

}
