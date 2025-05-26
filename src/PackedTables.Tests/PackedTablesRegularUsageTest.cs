using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackedTables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables.Models;
using PackedTables.Dictionaries;
using PackedTables.Extensions;
using System.ComponentModel.DataAnnotations;


namespace PackedTables.Tests {
  [TestClass]
  public class PackedTablesRegularUsageTest {

    public PackedTablesRegularUsageTest() {
     
    }

    [TestMethod]
    public void TestPackedTablesRegularUsage() {
      // Arrange
      var packedTables = new PackedTables();
      var settings = packedTables["Settings"];
      if (settings == null) {
        settings = packedTables.AddTable("Settings");
        settings.AddColumn("Key", (short)ColumnType.String);
        settings.AddColumn("Value", (short)ColumnType.String);
      }          

      packedTables.SaveTableToPackage(settings);

      var retrievedSettings = packedTables["Settings"];
      Assert.IsNotNull(retrievedSettings, "Settings table should not be null after saving.");
      Assert.IsTrue(retrievedSettings.Columns.Count == 2, "Settings table should have 2 columns after saving Key and Value.");

      string json = packedTables.SaveToJson();
      Console.WriteLine(json);
      Console.WriteLine(packedTables.SaveToBase64String());
      Assert.IsTrue(true);

    }

    [TestMethod]
    public void TestPackedTablesRegularUsage_MoreUsage() {
      // Arrange
      var packedTables = new PackedTables();
      var settings = packedTables["Settings"];
      if (settings == null) {
        settings = packedTables.AddTable("Settings");
        settings.AddColumn("Key", (short)ColumnType.String);
        settings.AddColumn("Value", (short)ColumnType.String);
      }

      var aRow = settings.AddRow();
      aRow["Key"].Value = "TestKey1";
      aRow["Value"].Value = "TestValue1";
      var aRow2 = settings.AddRow();
      aRow2["Key"].Value = "TestKey2";
      aRow2["Value"].Value = "TestValue2";
      packedTables.SaveTableToPackage(settings);


      var retrievedSettings = packedTables["Settings"];
      Assert.IsNotNull(retrievedSettings, "Settings table should not be null after saving.");
      Assert.IsTrue(retrievedSettings.Columns.Count == 2, "Settings table should have 2 columns after saving Key and Value.");
      Assert.AreEqual(2, retrievedSettings.Rows.Count, "Settings table should have 2 rows after saving.");

      string json = packedTables.SaveToJson();
      Console.WriteLine(json);
      Console.WriteLine(packedTables.SaveToBase64String());
      Assert.IsTrue(true);

    }

    [TestMethod]
    public void TestPackedTablesLoadFromBase64() {
      // Arrange
      
      var packedTables = new PackedTables();
      string base64Data = "lJKV2SQ0YzczMmZmMi1jMDc5LTQwNzctODBmMy02YzYzNjg5ZmYxNTDZJDkzNDFmMjcxLTAyNTYtNDUwOC05MDlkLTc4NmZhYjNhNGFiMAAIpVZhbHVlldkkY2NmY2NmYzItNzJjMy00ZDE3LTkyYTgtODI5OWFkZjliNmU22SQ5MzQxZjI3MS0wMjU2LTQ1MDgtOTA5ZC03ODZmYWIzYTRhYjAACKNLZXmQkJGS2SQ5MzQxZjI3MS0wMjU2LTQ1MDgtOTA5ZC03ODZmYWIzYTRhYjCoU2V0dGluZ3M="; 
      packedTables.LoadFromBase64String(base64Data);
      // Act
      var settings = packedTables["Settings"];
      
      // Assert
      Assert.IsNotNull(settings, "Settings table should not be null after loading from base64.");
      Assert.IsTrue(settings.Columns.Count == 2, "Settings table should have 2 columns after loading Key and Value.");
      Assert.AreEqual("Key", settings["Key"].ColumnName, "First column should be 'Key'.");
      Assert.AreEqual("Value", settings["Value"].ColumnName, "Second column should be 'Value'.");
     
    }

    [TestMethod]
    public void TestPackedTablesLoadFromBase64_AddsRows() {
      // Arrange

      var packedTables = new PackedTables();
      string base64Data = "lJKV2SQ0YzczMmZmMi1jMDc5LTQwNzctODBmMy02YzYzNjg5ZmYxNTDZJDkzNDFmMjcxLTAyNTYtNDUwOC05MDlkLTc4NmZhYjNhNGFiMAAIpVZhbHVlldkkY2NmY2NmYzItNzJjMy00ZDE3LTkyYTgtODI5OWFkZjliNmU22SQ5MzQxZjI3MS0wMjU2LTQ1MDgtOTA5ZC03ODZmYWIzYTRhYjAACKNLZXmQkJGS2SQ5MzQxZjI3MS0wMjU2LTQ1MDgtOTA5ZC03ODZmYWIzYTRhYjCoU2V0dGluZ3M="; 
      packedTables.LoadFromBase64String(base64Data);
      // Act
      var settings = packedTables["Settings"];

      // Assert
      Assert.IsNotNull(settings, "Settings table should not be null after loading from base64.");
      Assert.IsTrue(settings.Columns.Count == 2, "Settings table should have 2 columns after loading Key and Value.");
      Assert.AreEqual("Key", settings["Key"].ColumnName, "First column should be 'Key'.");
      Assert.AreEqual("Value", settings["Value"].ColumnName, "Second column should be 'Value'.");

      var newRow = settings.AddRow();
      newRow["Key"].Value = "NewKey";
      newRow["Value"].Value = "NewValue";
            
      settings.SaveToOwner();


      var retrievedSettings = packedTables["Settings"];
      Assert.IsNotNull(retrievedSettings, "Settings table should not be null after saving.");
      Assert.IsTrue(retrievedSettings.Columns.Count == 2, "Settings table should have 2 columns after saving Key and Value.");
      Assert.IsTrue(retrievedSettings.Rows.Count == 1, "Settings table should have one row after saving.");


      var newRow2 = retrievedSettings.Rows.AsList.FirstOrDefault();
      Assert.IsNotNull(newRow2, "New row should not be null.");
      Assert.AreEqual("NewKey", newRow2["Key"].Value, "New row's Key value should be 'NewKey'.");
      Assert.AreEqual("NewValue", newRow2["Value"].Value, "New row's Value value should be 'NewValue'.");


      var allRows = retrievedSettings.Rows.AsList;
      Assert.AreEqual(1, allRows.Count(), "There should be 1 row in the retrieved settings.");
      
      string json = packedTables.SaveToJson();
      Console.WriteLine(json);
      Console.WriteLine(packedTables.SaveToBase64String());
      Assert.IsTrue(true);


    }



  }
}
