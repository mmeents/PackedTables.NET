using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables.Dictionaries;
using PackedTables.Extensions;
using PackedTables.Models;

namespace PackedTables.Tests.Models
{
    [TestClass]
    public class TableModelTests
    {
        private PackedTables _packedTables;
        private TableModel _tableModel;

        
        public TableModelTests()
        {
            // Initialize mock PackedTables
            _packedTables = new PackedTables();
            var TableTest = _packedTables.AddTable("TestTable");
            TableTest.AddColumn("Column1", (short)ColumnType.String);
            TableTest.AddColumn("Column2", (short)ColumnType.Int32);
            var RowTest = TableTest.AddRow();
            RowTest["Column1"].Value = "TestValue1";
            RowTest["Column2"].Value = 123;

            _packedTables.SaveTableToPackage(TableTest);

            // Add mock table to the DataSetPackage
            _tableModel = _packedTables["TestTable"];
            
        }

        [TestMethod]
        public void TableModel_GetColumnID_ShouldReturnCorrectColumnID()
        {
            // Act
            var columnId = _tableModel.GetColumnID("Column1");

            // Assert
            Assert.AreNotEqual(Guid.Empty, columnId);
            Assert.AreEqual(_packedTables.Package.Columns.First(c => c.ColumnName == "Column1").Id, columnId);
        }

        [TestMethod]
        public void TableModel_GetColumnID_ShouldReturnEmptyGuidForNonExistentColumn()
        {
            // Act
            var columnId = _tableModel.GetColumnID("NonExistentColumn");

            // Assert
            Assert.AreEqual(Guid.Empty, columnId);
        }

        [TestMethod]
        public void TableModel_GetFieldsOfRow_ShouldReturnCorrectFields()
        {
            // Arrange
            var rowId = _packedTables.Package.Rows.First().Id;

            // Act
            var fields = _tableModel.GetFieldsOfRow(rowId);

            // Assert
            Assert.IsNotNull(fields);
            Assert.AreEqual(2, fields.Count());
            Assert.IsTrue(fields.Any(f => f.ValueString == "TestValue1"));
            Assert.IsTrue(fields.Any(f => f.ValueString == "123"));
        }

        [TestMethod]
        public void TableModel_Columns_ShouldBePopulatedCorrectly()
        {
            // Act
            var columns = _tableModel.Columns;

            // Assert
            Assert.IsNotNull(columns);
            Assert.AreEqual(2, columns.Count);
            Assert.IsTrue(columns.Values.Any(c => c.ColumnName == "Column1"));
            Assert.IsTrue(columns.Values.Any(c => c.ColumnName == "Column2"));
        }

        [TestMethod]
        public void TableModel_Rows_ShouldBePopulatedCorrectly()
        {
            // Act
            var rows = _tableModel.Rows;

            // Assert
            Assert.IsNotNull(rows);
            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual(_packedTables.Package.Rows.First().Id, rows.First().Key);
        }

        [TestMethod]
        public void TableModel_Fields_ShouldBePopulatedCorrectly()
        {
            // Act
            var fields = _tableModel.Fields;

            // Assert
            Assert.IsNotNull(fields);
            Assert.AreEqual(2, fields.Count);
            Assert.IsTrue(fields.Values.Any(f => f.ValueString == "TestValue1"));
            Assert.IsTrue(fields.Values.Any(f => f.ValueString == "123"));
        }
    }
}
