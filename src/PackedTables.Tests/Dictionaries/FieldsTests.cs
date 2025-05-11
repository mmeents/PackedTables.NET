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

        [TestInitialize]
        public void Setup()
        {
            // Initialize a mock Columns object with some test data
            _columns = new Columns();
            _columns.Add(new ColumnModel
            {
                Id = Guid.NewGuid(),
                ColumnName = "TestColumn1",
                ColumnType = (short)ColumnType.String
            });
            _columns.Add(new ColumnModel
            {
                Id = Guid.NewGuid(),
                ColumnName = "TestColumn2",
                ColumnType = (short)ColumnType.Int32
            });
        }

        [TestMethod]
        public void Fields_Add_ShouldAddField()
        {
            // Arrange
            var fields = new Fields(_columns);
            var field = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _columns.ByName("TestColumn1").Id,
                Value = "TestValue"
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
                ColumnId = _columns.AsList.First().Id,
                Value = "TestValue"
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
                ColumnId = _columns.AsList.First().Id,
                Value = "TestValue"
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
                ColumnId = _columns.AsList.First().Id,
                Value = "Value1"
            };
            var field2 = new FieldModel
            {
                RowId = Guid.NewGuid(),
                ColumnId = _columns.AsList.Last().Id,
                Value = "Value2"
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
                ColumnId = _columns.AsList.First().Id,
                ValueString = "TestValue"
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
                ColumnId = _columns.AsList.First().Id,
                ValueString = "TestValue"
            };

            // Act
            fields[field.Id] = field;

            // Assert
            Assert.IsTrue(fields.ContainsKey(field.Id));
            Assert.AreEqual(field, fields[field.Id]);
        }
    }
}
