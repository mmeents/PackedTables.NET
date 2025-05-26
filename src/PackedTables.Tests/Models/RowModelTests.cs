using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables.Dictionaries;
using PackedTables.Models;

namespace PackedTables.Tests.Models
{
    [TestClass]
    public class RowModelTests
    {
        private TableModel _tableModel;
        private Columns _columns;
        private Fields _fields;

        
        public RowModelTests()
        {
            // Initialize mock Columns and Fields
            _columns = new Columns();
            var column1 = new ColumnModel
            {
                Id = Guid.NewGuid(),
                ColumnName = "Column1",
                ColumnType = (short)ColumnType.String
            };
            var column2 = new ColumnModel
            {
                Id = Guid.NewGuid(),
                ColumnName = "Column2",
                ColumnType = (short)ColumnType.Int32
            };
            _columns.Add(column1);
            _columns.Add(column2);

            _fields = new Fields(_columns);

            // Initialize mock TableModel
            _tableModel = new TableModel
            {
                Id = Guid.NewGuid(),
                Name = "TestTable",
                Columns = _columns,
                Fields = _fields,                
            };
            _tableModel.Rows = new Rows(_tableModel);
        }

        [TestMethod]
        public void RowModel_DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var row = new RowModel();

            // Assert
            Assert.AreEqual(Guid.Empty, row.Id);
            Assert.AreEqual(Guid.Empty, row.TableId);
            Assert.IsNull(row.Owner);
            Assert.IsNull(row.RowFields);
        }

        [TestMethod]
        public void RowModel_Indexer_Get_ShouldReturnField()
        {

            // Arrange            
            var field = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _tableModel.GetColumnID("Column1"),
                ValueType = ColumnType.String,
                Value = "TestValue"
            };
            _fields.Add(field);
            var row = new RowModel(_tableModel, _fields) {
              RowFields = _fields
            };

            // Act
            var retrievedField = row["Column1"];

            // Assert
            Assert.IsNotNull(retrievedField);
            Assert.AreEqual(field, retrievedField);
        }

        [TestMethod]
        public void RowModel_Indexer_Get_ShouldReturnNullForNonExistentColumn()
        {
            // Arrange
            var row = new RowModel(_tableModel, _fields);

            // Act
            var retrievedField = row["NonExistentColumn"];

            // Assert
            Assert.IsNull(retrievedField);
        }

        [TestMethod]
        public void RowModel_Indexer_Set_ShouldAddField()
        {
            // Arrange
            var row = new RowModel(_tableModel, _fields);
            var field = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _tableModel.GetColumnID("Column1"),
                ValueType = ColumnType.String,
                ValueString = "TestValue"
            };

            // Act
            row["Column1"] = field;

            // Assert
            Assert.IsTrue(_fields.ContainsKey(field.Id));
            Assert.AreEqual(field, _fields[field.Id]);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Column not found")]
        public void RowModel_Indexer_Set_ShouldThrowExceptionForInvalidColumn()
        {
            // Arrange
            var row = new RowModel(_tableModel, _fields);
            var field = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = Guid.NewGuid(), // Invalid ColumnId
                ValueType = ColumnType.String,
                ValueString = "TestValue"
            };

            // Act
            row["NonExistentColumn"] = field;
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "ColumnId mismatch")]
        public void RowModel_Indexer_Set_ShouldThrowExceptionForColumnIdMismatch()
        {
            // Arrange
            var row = new RowModel(_tableModel, _fields);
            var field = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = Guid.NewGuid(), // Mismatched ColumnId
                ValueType = ColumnType.String,
                ValueString = "TestValue"
            };

            // Act
            row["Column1"] = field;
        }

        [TestMethod]
        public void RowModel_Indexer_Set_ShouldRemoveField()
        {
            // Arrange            
            var field = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _tableModel.GetColumnID("Column1"),
                ValueString = "TestValue"
            };
            _fields.Add(field);
            var row = new RowModel(_tableModel, _fields); 

            // Act
            row["Column1"] = null;

            // Assert
            Assert.IsFalse(_fields.Values.Any(f => f.ColumnId == field.ColumnId));
        }
    }
}
