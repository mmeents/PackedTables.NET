using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables.Dictionaries;
using PackedTables.Models;

namespace PackedTables.Tests.Dictionaries
{
    [TestClass]
    public class RowsTests
    {
        private PackedTables _packedTables;
        private Tables _tables;
        private TableModel _tableModel;
        private Columns _columns;
        private Fields _fields;
       
        public RowsTests()
        {
            _columns = new Columns();
            var column1 = new ColumnModel {
              Id = Guid.NewGuid(),
              ColumnName = "Column1",
              ColumnType = (short)ColumnType.String
            };
            var column2 = new ColumnModel {
              Id = Guid.NewGuid(),
              ColumnName = "Column2",
              ColumnType = (short)ColumnType.Int32
            };
            _columns.Add(column1);
            _columns.Add(column2);

            _fields = new Fields(_columns);
            // Initialize a mock TableModel
            _tableModel = new TableModel
            {
                Id = Guid.NewGuid(),
                Name = "TestTable"
            };         
            
            _packedTables = new PackedTables();
            _packedTables.Package.Columns = _columns.AsList;
            _packedTables.Package.Fields = _fields.AsList;      
            _packedTables.Package.Tables = new List<TableModel>(){ _tableModel };           

        }

        [TestMethod]
        public void Rows_Add_ShouldAddRow()
        {
            // Arrange
            _tableModel.Rows = new Rows(_tableModel);
            var row = new RowModel(_tableModel, _tableModel.Fields) { TableId = _tableModel.Id };
            row.RowFields = new Fields(_packedTables.Fields.AsList.Where(x => x.RowId == row.Id), row, _tableModel.Columns);

            // Act
            var addedRow = _tableModel.Rows.Add(row);

            // Assert
            Assert.IsTrue(_tableModel.Rows.ContainsKey(addedRow.Id));
            Assert.AreEqual(_tableModel, addedRow.Owner);
            Assert.IsNotNull(addedRow.RowFields);
        }

        [TestMethod]
        public void Rows_Remove_ById_ShouldRemoveRow()
        {
            // Arrange
            var rows = new Rows(_tableModel );
            var row = new RowModel(_tableModel, _tableModel.Fields) { TableId = _tableModel.Id };
            row.RowFields = new Fields(_packedTables.Fields.AsList.Where(x => x.RowId == row.Id), row, _tableModel.Columns);

            var addedRow = rows.Add(row);

            // Act
            rows.Remove(addedRow.Id);

            // Assert
            Assert.IsFalse(rows.ContainsKey(addedRow.Id));
        }

        [TestMethod]
        public void Rows_Remove_ByRow_ShouldRemoveRow()
        {
            // Arrange
            var rows = new Rows(_tableModel);
            var row = new RowModel(_tableModel, _tableModel.Fields) { TableId = _tableModel.Id };
            row.RowFields = new Fields(_packedTables.Fields.AsList.Where(x => x.RowId == row.Id), row, _tableModel.Columns);
            var addedRow = rows.Add(row);

            // Act
            rows.Remove(addedRow);

            // Assert
            Assert.IsFalse(rows.ContainsKey(addedRow.Id));
        }

        [TestMethod]
        public void Rows_AsList_ShouldReturnAllRows()
        {
            // Arrange
            var rows = new Rows(_tableModel);            
            var row1 = new RowModel(_tableModel, _tableModel.Fields) { TableId = _tableModel.Id };
            row1.RowFields = new Fields(_packedTables.Fields.AsList.Where(x => x.RowId == row1.Id), row1, _tableModel.Columns);
                  
            var row2 = new RowModel(_tableModel, _tableModel.Fields) { TableId = _tableModel.Id };
            row2.RowFields = new Fields(_packedTables.Fields.AsList.Where(x => x.RowId == row2.Id), row2, _tableModel.Columns);

            rows.Add(row1);
            rows.Add(row2);

            // Act
            var asList = rows.AsList;

            // Assert
            Assert.AreEqual(2, asList.Count());
            Assert.IsTrue(asList.Contains(row1));
            Assert.IsTrue(asList.Contains(row2));
        }

        [TestMethod]
        public void Rows_AsList_Set_ShouldAddRows()
        {
            // Arrange
            var rows = new Rows(_tableModel);
            var row1 = new RowModel(_tableModel, _tableModel.Fields) { TableId = _tableModel.Id };
            row1.RowFields = new Fields(_packedTables.Fields.AsList.Where(x => x.RowId == row1.Id), row1, _tableModel.Columns);

            var row2 = new RowModel(_tableModel, _tableModel.Fields) { TableId = _tableModel.Id };
            row2.RowFields = new Fields(_packedTables.Fields.AsList.Where(x => x.RowId == row2.Id), row2, _tableModel.Columns);

            rows.Add(row1);
            rows.Add(row2);
            var rowList = new List<RowModel> { row1, row2 };

            // Act
            rows.AsList = rowList;

            // Assert
            Assert.AreEqual(2, rows.Count);
            Assert.IsTrue(rows.Values.Contains(row1));
            Assert.IsTrue(rows.Values.Contains(row2));
        }

        [TestMethod]
        public void Rows_Indexer_Get_ShouldReturnRow()
        {
            // Arrange
            var rows = new Rows(_tableModel);
            var row = new RowModel(_tableModel, _tableModel.Fields) { TableId = _tableModel.Id };
            row.RowFields = new Fields(_packedTables.Fields.AsList.Where(x => x.RowId == row.Id), row, _tableModel.Columns);
            var addedRow = rows.Add(row);

            // Act
            var retrievedRow = rows[addedRow.Id];

            // Assert
            Assert.AreEqual(addedRow, retrievedRow);
        }

        [TestMethod]
        public void Rows_Indexer_Set_ShouldAddOrUpdateRow()
        {
            // Arrange
            var rows = new Rows(_tableModel);
            var row = new RowModel(_tableModel, _tableModel.Fields) { TableId = _tableModel.Id };
            row.RowFields = new Fields(_packedTables.Fields.AsList.Where(x => x.RowId == row.Id), row, _tableModel.Columns);

            // Act
            rows[row.Id] = row;

            // Assert
            Assert.IsTrue(rows.ContainsKey(row.Id));
            Assert.AreEqual(row, rows[row.Id]);
        }
    }
}
