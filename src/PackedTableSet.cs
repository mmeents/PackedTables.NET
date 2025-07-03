using System.Collections.Generic;

namespace PackedTables.Net
{
  // 
  public class PackedTableSet {
    private string _FileName { get; set; } = "";
    private bool _Modified { get; set; } = false;
    public bool Modified {
      get { return _Modified; }
      set { _Modified = value; }
    }
    private DataSetModel _Package { get; set; } = new DataSetModel();

    public PackedTableSet() {}
    public PackedTableSet(DataSetModel package) {
      _Package = package;
    }
    public PackedTableSet(string fileName) {
      _Package = new DataSetModel();
      LoadFromFile(fileName);
    }
    #region Load Save 
    public string SaveToBase64String() {
      var encoded = MessagePack.MessagePackSerializer.Serialize(_Package);
      var base64 = Convert.ToBase64String(encoded);
      return base64;
    }

    public void LoadFromBase64String(string base64) {
      if (base64 == null || base64.Length == 0) {
        _Package = new DataSetModel();
      } else {
        var decoded = Convert.FromBase64String(base64);
        _Package = MessagePack.MessagePackSerializer.Deserialize<DataSetModel>(decoded);
        if(_Package != null) {
          ResetOwnership();
        }
      }
      _Modified = false;
    }

    private void ResetOwnership() {
      foreach (var table in _Package.Tables.Values) {        
        foreach (var row in table.Rows.Values) {
          row.Owner = table; // route to the table for columns
          foreach (var field in row.RowFields.Values) {
            field.OwnerRow = row; // Set the owner of each field to the row
          }
        }
        table.Owner = this; // Set the owner of each table to this instance also rebuilds indexes.
      }
    }

    public void LoadFromFile(string fileName) {
      if (File.Exists(fileName)) {
        _FileName = fileName;
        _Modified = false;
        var encoded = Task.Run(async () => await fileName.ReadAllTextAsync().ConfigureAwait(false)).GetAwaiter().GetResult();
        LoadFromBase64String(encoded);
      }
    }

    public void SaveToFile(string fileName) {
      var base64 = SaveToBase64String();
      Task.Run(async () => await base64.WriteAllTextAsync(fileName).ConfigureAwait(false)).GetAwaiter().GetResult();
      _Modified = false;
    }

    public string SaveToJson() {
      return MessagePack.MessagePackSerializer.SerializeToJson(_Package);
    }

    public void LoadFromJson(string json) {
      if (json == null || json.Length == 0) {
        _Package = new DataSetModel();
      } else {
        var byteArray = MessagePack.MessagePackSerializer.ConvertFromJson(json);
        _Package = MessagePack.MessagePackSerializer.Deserialize<DataSetModel>(byteArray);
        if (_Package != null) {
          ResetOwnership();
        }
      }
      _Modified = false;
    }
    #endregion

    public int TableCount {
      get { return _Package.Tables.Count; }
    }

    public TableModel? this[string tableName] {
      get {
        var table = GetTableByName(tableName);                
        return table;
      }
      set {
        var table = GetTableByName(tableName);
        if (table != null) {
          _Package.Tables.TryRemove(table.Id, out _);          
        }
        if (value != null) {
          _Package.Tables[value.Id] = value;
        }
      }
    }

    public TableModel AddTable(string tableName) {
      var table = GetTableByName(tableName);
      if (table != null) throw new Exception("Table {tableName} already exists");
      var tableNew = new TableModel() {
        Id = GetNextTableId(),
        Name = tableName,
        Owner = this
      };
      _Package.Tables[tableNew.Id] = tableNew;
      _Package.NameIndex[tableName] = tableNew.Id;
      _Modified = true;
      return tableNew;
    }

    public TableModel? GetTableByName(string tableName) {        
      if (_Package.NameIndex.TryGetValue(tableName, out var tableId)) {
        return _Package.Tables[tableId];
      }    
      return null;     
    }

    private int GetNextTableId() {
      if (_Package.Tables.Count == 0) return 1;
      var max = _Package.Tables.Keys.Max(x => x);
      return (int)(max + 1);
    } 

    public void RemoveTable(string tableName) {
      var table = GetTableByName(tableName);
      if (table != null) {
        _Package.Tables.TryRemove(table.Id, out _);
        _Package.NameIndex.TryRemove(tableName, out _);
        _Modified = true;
      } else {
        throw new Exception($"Table {tableName} does not exist.");
      }
    }

    public IEnumerable<TableModel> GetTables() {
      return _Package.Tables.Values;
    }

  }
}
