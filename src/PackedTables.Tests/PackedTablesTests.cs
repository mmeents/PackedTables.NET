using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables;
using PackedTables.Dictionaries;
using PackedTables.Models;

namespace PackedTables.Tests
{
    [TestClass]
    public class PackedTablesTests
    {
        private PackedTables _packedTables;
        
        public PackedTablesTests()
        {
            // Initialize PackedTables with a mock DataSetPackage
            _packedTables = new PackedTables();

            // Add mock columns to the DataSetPackage
            var column1 = new ColumnModel
            {
                Id = Guid.NewGuid(),
                TableId = Guid.NewGuid(),
                ColumnName = "Column1",
                ColumnType = (short)ColumnType.String
            };
            var column2 = new ColumnModel
            {
                Id = Guid.NewGuid(),
                TableId = column1.TableId,
                ColumnName = "Column2",
                ColumnType = (short)ColumnType.Int32
            };
            _packedTables.Package.Columns = new List<ColumnModel> { column1, column2 };

            // Add mock fields to the DataSetPackage
            var field1 = new FieldModel
            {
                Id = Guid.NewGuid(),
                RowId = Guid.NewGuid(),
                ColumnId = column1.Id,
                ValueString = "TestValue1"
            };
            var field2 = new FieldModel
            {
                Id = Guid.NewGuid(),
                RowId = field1.RowId,
                ColumnId = column2.Id,
                ValueString = "123"
            };
            _packedTables.Package.Fields = new List<FieldModel> { field1, field2 };

            // Add mock rows to the DataSetPackage
            var row = new RowModel
            {
                Id = field1.RowId,
                TableId = column1.TableId
            };
            _packedTables.Package.Rows = new List<RowModel> { row };

            // Add mock table to the DataSetPackage
            var table = new TableModel
            {
                Id = column1.TableId,
                Name = "TestTable"
            };
            _packedTables.Package.Tables = new List<TableModel> { table };
        }

        [TestMethod]
        public void PackedTables_Columns_ShouldReturnCorrectColumns()
        {
            // Act
            var columns = _packedTables.Columns;

            // Assert
            Assert.IsNotNull(columns);
            Assert.AreEqual(2, columns.Count);
            Assert.IsTrue(columns.Values.Any(c => c.ColumnName == "Column1"));
            Assert.IsTrue(columns.Values.Any(c => c.ColumnName == "Column2"));
        }

        [TestMethod]
        public void PackedTables_Fields_ShouldReturnCorrectFields()
        {
            // Act
            var fields = _packedTables.Fields;

            // Assert
            Assert.IsNotNull(fields);
            Assert.AreEqual(2, fields.Count);
            Assert.IsTrue(fields.Values.Any(f => f.ValueString == "TestValue1"));
            Assert.IsTrue(fields.Values.Any(f => f.ValueString == "123"));
        }

        [TestMethod]
        public void PackedTables_Tables_ShouldReturnCorrectTables()
        {
            // Act
            var tables = _packedTables.Tables;

            // Assert
            Assert.IsNotNull(tables);
            Assert.AreEqual(1, tables.Count);
            Assert.IsTrue(tables.Values.Any(t => t.Name == "TestTable"));
        }

        [TestMethod]
        public void PackedTables_GetColumnsOfTable_ShouldReturnCorrectColumns()
        {
            // Arrange
            var tableId = _packedTables.Package.Tables.First().Id;

            // Act
            var columns = _packedTables.GetColumnsOfTable(tableId);

            // Assert
            Assert.IsNotNull(columns);
            Assert.AreEqual(2, columns.Count);
            Assert.IsTrue(columns.Values.Any(c => c.ColumnName == "Column1"));
            Assert.IsTrue(columns.Values.Any(c => c.ColumnName == "Column2"));
        }

        [TestMethod]
        public void PackedTables_PopulateTable_ShouldPopulateCorrectly()
        {
            // Arrange
            var tableId = _packedTables.Package.Tables.First().Id;
            var table = _packedTables.Tables[tableId];

            // Act
            var populatedTable = _packedTables.PopulateTable(table);

            // Assert
            Assert.IsNotNull(populatedTable.Columns);
            Assert.AreEqual(2, populatedTable.Columns.Count);
            Assert.IsNotNull(populatedTable.Rows);
            Assert.AreEqual(1, populatedTable.Rows.Count);
            Assert.IsNotNull(populatedTable.Fields);
            Assert.AreEqual(2, populatedTable.Fields.Count);
        }
    }
}
