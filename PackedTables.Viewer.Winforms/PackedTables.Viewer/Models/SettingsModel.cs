using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Viewer.Models {
  public class SettingsModel : PackedTableSet {    
    private string _fileName = "";
    private bool _valuesModified = false;
    private TableModel? _settings = null;
    private TableModel? _recentlyUsed = null;
    public SettingsModel(string fileName) : base(fileName) {      
      _fileName = fileName;
      this.LoadFromFile(fileName);
      this.EnsureDefaults();
    }
    private void EnsureDefaults() {
      _settings = this["Settings"];
      if (_settings == null) {
        _settings = this.AddTable("Settings");
        _settings.AddColumn("Key", ColumnType.String);
        _settings.AddColumn("Value", ColumnType.String);
        _valuesModified = true; 
      }
      _recentlyUsed = this["RecentlyUsed"];
      if (_recentlyUsed == null) {
        _recentlyUsed = this.AddTable("RecentlyUsed");
        _recentlyUsed.AddColumn("FileName", ColumnType.String);
        _recentlyUsed.AddColumn("LastUsed", ColumnType.DateTime);
        _valuesModified = true;
      }    
    }

    public void Save() {       
      this.SaveToFile(_fileName);
      _valuesModified = false;      
    }

    public TableModel Settings { 
      get { return _settings; }
      set {
        if (value == null) throw new ArgumentNullException(nameof(value), "Settings cannot be null.");
        _settings = value; 
        _valuesModified = true;       
      }
    }
    public TableModel RecentlyUsed { 
      get { return _recentlyUsed; }
      set { 
        if (value == null) throw new ArgumentNullException(nameof(value), "RecentlyUsed cannot be null.");
        _recentlyUsed = value; 
        _valuesModified = true;
      }
    }

    


  }
}
