using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables.Dictionaries;
using PackedTables.Models;

namespace PackedTables.Tests.Dictionaries
{
    [TestClass]
    public class FieldsTests
    {
        private Columns _columns;
        private ColumnModel _column1;
        private ColumnModel _column2;

        public FieldsTests()
        {
            Guid tableId = Guid.NewGuid();
            // Initialize a mock Columns object with some test data
            _columns = new Columns();
            
            _column1 = new ColumnModel
            {
                Id = Guid.NewGuid(),
                TableId = tableId,
                ColumnName = "TestColumn1",
                ColumnType = (short)ColumnType.String
            };

            _column2 = new ColumnModel
            {
                Id = Guid.NewGuid(),
                TableId = tableId,
                ColumnName = "TestColumn2",
                ColumnType = (short)ColumnType.Int32
            };
            _columns.Add(_column1);
            _columns.Add(_column2);
        }

        [TestMethod]
        public void Fields_Add_ShouldAddField()
        {
            // Arrange
            var fields = new Fields(_columns);
            var field = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _column1.Id,
                ValueType = ColumnType.String,
                ValueString = "TestValue"
            };

            // Act
            var addedField = fields.Add(field);

            // Assert
            Assert.IsTrue(fields.ContainsKey(addedField.Id));
            Assert.AreEqual(field.ValueString, addedField.ValueString);
            Assert.AreEqual(ColumnType.String, addedField.ValueType);
        }

        [TestMethod]
        public void Fields_Remove_ById_ShouldRemoveField()
        {
            // Arrange
            var fields = new Fields(_columns);
            var field = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _column1.Id,
                ValueType = ColumnType.String,
                ValueString = "TestValue"
            };
            var addedField = fields.Add(field);

            // Act
            fields.Remove(addedField.Id);

            // Assert
            Assert.IsFalse(fields.ContainsKey(addedField.Id));
        }

        [TestMethod]
        public void Fields_Remove_ByField_ShouldRemoveField()
        {
            // Arrange
            var fields = new Fields(_columns);
            var field = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _column1.Id,
                ValueType = ColumnType.String,
                ValueString = "TestValue"
            };
            var addedField = fields.Add(field);

            // Act
            fields.Remove(addedField);

            // Assert
            Assert.IsFalse(fields.ContainsKey(addedField.Id));
        }

        [TestMethod]
        public void Fields_AsList_ShouldReturnAllFields()
        {
            // Arrange
            var fields = new Fields(_columns);
            var field1 = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _column1.Id,
                ValueType = ColumnType.String,
                Value = "Value1"
            };
            var field2 = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _column2.Id,
                ValueType = ColumnType.Int32,
                Value = 122
            };
            fields.Add(field1);
            fields.Add(field2);

            // Act
            var asList = fields.AsList;

            // Assert
            Assert.AreEqual(2, asList.Count());
            Assert.IsTrue(asList.Contains(field1));
            Assert.IsTrue(asList.Contains(field2));
        }

        [TestMethod]
        public void Fields_GetNextId_ShouldReturnUniqueId()
        {
            // Arrange
            var fields = new Fields(_columns);

            // Act
            var id1 = fields.GetNextId();
            var id2 = fields.GetNextId();

            // Assert
            Assert.AreNotEqual(id1, id2);
        }

        [TestMethod]
        public void Fields_Indexer_Get_ShouldReturnField()
        {
            // Arrange
            var fields = new Fields(_columns);
            var field = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _column1.Id,
                ValueType = ColumnType.String, ValueString = "TestValue"
            };
            var addedField = fields.Add(field);

            // Act
            var retrievedField = fields[addedField.Id];

            // Assert
            Assert.AreEqual(addedField, retrievedField);
        }

        [TestMethod]
        public void Fields_Indexer_Set_ShouldAddOrUpdateField()
        {
            // Arrange
            var fields = new Fields(_columns);
            var field = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _column1.Id,
                ValueType = ColumnType.String, ValueString = "TestValue"
            };

            // Act
            fields[field.Id] = field;

            // Assert
            Assert.IsTrue(fields.ContainsKey(field.Id));
            Assert.AreEqual(field, fields[field.Id]);
        }
    }
}
