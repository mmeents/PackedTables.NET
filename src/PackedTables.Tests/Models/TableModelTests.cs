using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables.Dictionaries;
using PackedTables.Models;

namespace PackedTables.Tests.Models
{
    [TestClass]
    public class TableModelTests
    {
        private PackedTables _packedTables;
        private TableModel _tableModel;

        [TestInitialize]
        public void Setup()
        {
            // Initialize mock PackedTables
            _packedTables = new PackedTables();
            var tableID = Guid.NewGuid();

            // Add mock columns to the DataSetPackage
            var column1 = new ColumnModel
            {
                Id = Guid.NewGuid(),
                TableId = tableID,
                ColumnName = "Column1",
                ColumnType = (short)ColumnType.String
            };
            var column2 = new ColumnModel
            {
                Id = Guid.NewGuid(),
                TableId = tableID,
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
                TableId = tableID
            };
            _packedTables.Package.Rows = new List<RowModel> { row };

            // Add mock table to the DataSetPackage
            _tableModel = new TableModel()
            {
                Id = tableID,
                Name = "TestTable",              
                Owner = _packedTables.Tables
            };
            _packedTables.Tables.Add( _tableModel) ;
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
