using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackedTables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables.Net;
using System.ComponentModel.DataAnnotations;


namespace PackedTables.Tests {
  [TestClass]
  public class PackedTablesRegularUsageTest {

    public PackedTablesRegularUsageTest() {
     
    }

    [TestMethod]
    public void TestPackedTablesRegularUsage() {
      // Arrange
      var packedTables = new PackedTableSet();
      var settings = packedTables["Settings"];               // This is a lookup, if the table does not exist, it will return null.
      if (settings == null) {
        settings = packedTables.AddTable("Settings");         // This will create a new table with the name "Settings".
        settings.AddColumn("Key", ColumnType.String);  // Adding a column named "Key" of type String.
        settings.AddColumn("Value", ColumnType.String);// Adding a column named "Value" of type String. 
      }               

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
      var packedTables = new PackedTableSet();
      var settings = packedTables["Settings"];
      if (settings == null) {
        settings = packedTables.AddTable("Settings");
        settings.AddColumn("Key", ColumnType.String);
        settings.AddColumn("Value", ColumnType.String);
      }

      var aRow = settings.AddRow();
      aRow["Key"].Value = "TestKey1";
      aRow["Value"].Value = "TestValue1";
      var aRow2 = settings.AddRow();
      aRow2["Key"].Value = "TestKey2";
      aRow2["Value"].Value = "TestValue2";
      settings.Post();


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
      
      var packedTables = new PackedTableSet();
      string base64Data = "koEBlgGoU2V0dGluZ3OCAZwBAQEIo0tlecLDAKCgoKACnAIBAgilVmFsdWXCwwCgoKCgggGTAQGCAZUBAQGoVGVzdEtleTEIApUCAQKqVGVzdFZhbHVlMQgCkwIBggGVAQIBqFRlc3RLZXkyCAKVAgICqlRlc3RWYWx1ZTIIoMOBqFNldHRpbmdzAQ=="; 
      packedTables.LoadFromBase64String(base64Data);
      // Act
      var settings = packedTables["Settings"];
      
      // Assert
      Assert.IsNotNull(settings, "Settings table should not be null after loading from base64.");
      Assert.IsTrue(settings.Columns.Count == 2, "Settings table should have 2 columns after loading Key and Value.");
      Assert.AreEqual("Key", settings.Columns[1].ColumnName, "First column should be 'Key'.");
      Assert.AreEqual("Value", settings.Columns[2].ColumnName, "Second column should be 'Value'.");
     
    }

    [TestMethod]
    public void TestPackedTablesLoadFromBase64_AddsRows() {
      // Arrange

      var packedTables = new PackedTableSet();
      string base64Data = "koEBlgGoU2V0dGluZ3OCAZwBAQEIo0tlecLDAKCgoKACnAIBAgilVmFsdWXCwwCgoKCgggGTAQGCAZUBAQGoVGVzdEtleTEIApUCAQKqVGVzdFZhbHVlMQgCkwIBggGVAQIBqFRlc3RLZXkyCAKVAgICqlRlc3RWYWx1ZTIIoMOBqFNldHRpbmdzAQ=="; 
      packedTables.LoadFromBase64String(base64Data);
      // Act
      var settings = packedTables["Settings"];

      // Assert
      Assert.IsNotNull(settings, "Settings table should not be null after loading from base64.");
      Assert.IsTrue(settings.Columns.Count == 2, "Settings table should have 2 columns after loading Key and Value.");
      Assert.AreEqual("Key", settings.Columns[1].ColumnName, "First column should be 'Key'.");
      Assert.AreEqual("Value", settings.Columns[2].ColumnName, "Second column should be 'Value'.");

      var newRow = settings.AddRow();
      newRow["Key"].Value = "NewKey";
      newRow["Value"].Value = "NewValue";
      settings.Post(); // Post the changes to the table

      var retrievedSettings = packedTables["Settings"];
      Assert.IsNotNull(retrievedSettings, "Settings table should not be null after saving.");
      Assert.IsTrue(retrievedSettings.Columns.Count == 2, "Settings table should have 2 columns after saving Key and Value.");
      Assert.IsTrue(retrievedSettings.Rows.Count == 3, "Settings table should have one row after saving.");


      var newRow2 = retrievedSettings.Current;
      Assert.IsNotNull(newRow2, "New row should not be null.");
      Assert.AreEqual("NewKey", newRow2["Key"].Value, "New row's Key value should be 'NewKey'.");
      Assert.AreEqual("NewValue", newRow2["Value"].Value, "New row's Value value should be 'NewValue'.");


      var allRows = retrievedSettings.Rows.Values;
      Assert.AreEqual(3, allRows.Count(), "There should be 3 rows in the retrieved settings.");
      
      string json = packedTables.SaveToJson();
      Console.WriteLine(json);
      Console.WriteLine(packedTables.SaveToBase64String());
      Assert.IsTrue(true);


    }

   
  }
}
