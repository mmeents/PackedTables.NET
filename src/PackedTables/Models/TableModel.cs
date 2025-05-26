using MessagePack;
using PackedTables.Dictionaries;
using PackedTables.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Models {

  [MessagePackObject]
  public class TableModel {

    public TableModel() {
      Id = Guid.Empty;
      Name = string.Empty;
      LoadFromNoOwner();
    }
    public TableModel(Tables owner, Guid id) {
      Owner = owner;
      Id = id;
      var table = owner.Owner!.Package.Tables.FirstOrDefault(x => x.Id == id);
      if (table != null) {
        Name = table.Name;
        LoadFromOwner(table.Id);
      } else {
        Name = string.Empty;
        LoadFromNoOwner();
      }
    }
    public TableModel(Tables owner, string name) {
      Owner = owner;
      Id = Guid.Empty;
      Name = name;
      var OwnerHasTable = owner.Owner!.Package.Tables.FirstOrDefault(x => x.Name == name);
      if (OwnerHasTable != null) {
        LoadFromOwner(OwnerHasTable.Id);
      } else {
        LoadFromNoOwner();
        this.Id = Guid.NewGuid();
      }
    }

    [Key(0)]
    public Guid Id { get; set; } = Guid.Empty;

    [Key(1)]
    public string Name { get; set; } = "";

    [IgnoreMember]
    public Tables Owner { get; set; } = null!;

    [IgnoreMember]
    public Columns Columns { get; set; } = null!;

    [IgnoreMember] 
    public Fields Fields { get; set; } = null!;

    [IgnoreMember]
    public Rows Rows { get; set; } = null!;

    public ColumnModel? this[string columnName] {
      get {
        if (Columns == null || Columns.Count == 0) return null;
        var column = Columns.ByName(columnName);                
        return column;
      }
    }

    public Guid GetColumnID(string columnName) {
      if (Columns == null || Columns.Count == 0) return Guid.Empty;
      var column = Columns.FirstOrDefault(x => x.Value.ColumnName == columnName);
      if (column.Key != Guid.Empty) {
        return column.Key;
      }
      return Guid.Empty;
    }

    public IEnumerable<FieldModel> GetFieldsOfRow(Guid rowId) {
      List<FieldModel> fields = new List<FieldModel>();
      if (Fields != null && Fields.Count > 0) { 
        fields.AddRange(Fields.AsList.Where(x => x.RowId == rowId).ToList());
      }       
      if (!Rows.ContainsKey(rowId))  return fields;
      if (Rows[rowId].RowFields != null && Rows[rowId].RowFields.Count >= 0) {
        fields.AddRange(Rows[rowId].RowFields.AsList);
      }      
      if (Owner == null) return fields;
      var allFields =  Owner[Id].Fields;
      allFields.Synchronize(Fields);
      return allFields.Select(kvp => kvp.Value).Where(x => x.RowId == rowId);
    }

    public void SaveToOwner() { 
      if (Owner == null) return;
      if (Owner.Owner == null) return;
      Owner.Owner.SaveTableToPackage(this);        
    }

    public void LoadFromOwner(Guid tableId) {
      if (Owner == null) return;
      if (Owner.Owner == null) return;
      this.Columns = Owner.Owner.GetColumnsOfTable(tableId);
      this.Fields = Owner.Owner.GetFieldsOfTable(tableId);
      this.Rows = Owner.Owner.GetRowsOfTable(tableId);
    }
    private void LoadFromNoOwner() { 
      this.Columns = new Columns();
      this.Fields = new Fields(this.Columns);
      this.Rows = new Rows(this);
    }

    public ColumnModel AddColumn(string columnName, short columnType) {
      var column = new ColumnModel() {
        Id = Guid.NewGuid(),
        TableId = this.Id,
        ColumnName = columnName,
        ColumnType = columnType
      };
      this.Columns.Add(column);
      foreach (var row in this.Rows.AsList) {
        this.Fields.Add(new FieldModel() {
          Id = Guid.NewGuid(),
          RowId = row.Id,
          ColumnId = column.Id,
          ValueType = (ColumnType)column.ColumnType
        });        
      }
      return column;
    }

    /// <summary>
    /// Each row has fields, but the table also has fields that the rows build from. The notify is to propagate the changes from the row to the table's fields.
    /// </summary>
    /// <param name="rowId"></param>    
    public void NotifyValueChanged(Guid rowId) {
      if (Owner == null) return;
      var fields = this.Rows[rowId].RowFields;
      this.Fields.Synchronize(fields);      
    }

    public void RemoveColumn(Guid columnId) {
      if (this.Columns.TryGetValue(columnId, out ColumnModel? column)) {
        foreach (var row in this.Rows.AsList) {
          var field = this.Fields.Values.FirstOrDefault(x => x.RowId == row.Id && x.ColumnId == column.Id);
          if (field != null) {
            this.Fields.Remove(field);
          }
        }
        this.Columns.Remove(columnId);
      }
    }

    public RowModel AddRow() {
      if (Owner == null) throw new InvalidOperationException("Owner cannot be null when adding a row.");
      TableModel? ownerTable = Owner[Id];      
      var fields = new Fields(ownerTable.Columns);
      var newRow = new RowModel(ownerTable, fields);      
      return this.Rows.Add(newRow); 
    }
    
    public void RemoveRow(Guid rowId) {
      if (this.Rows.TryGetValue(rowId, out RowModel? row)) {
        foreach (var field in this.Fields.Values.Where(f => f.RowId == rowId).ToList()) {
          this.Fields.Remove(field);
        }
        this.Rows.Remove(rowId);
      }
    }
    
  }
}
