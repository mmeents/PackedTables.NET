using System;
using MessagePack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables.Models;

namespace PackedTables.Tests.Models
{
    [TestClass]
    public class ColumnModelTests
    {
        [TestMethod]
        public void ColumnModel_DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var column = new ColumnModel();

            // Assert
            Assert.AreEqual(Guid.Empty, column.Id);
            Assert.AreEqual(Guid.Empty, column.TableId);
            Assert.AreEqual(0, column.Rank);
            Assert.AreEqual(0, column.ColumnType);
            Assert.AreEqual(string.Empty, column.ColumnName);
        }

        [TestMethod]
        public void ColumnModel_PropertyAssignment_ShouldWorkCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var tableId = Guid.NewGuid();
            var rank = 1;
            var columnType = (short)ColumnType.String;
            var columnName = "TestColumn";

            // Act
            var column = new ColumnModel
            {
                Id = id,
                TableId = tableId,
                Rank = rank,
                ColumnType = columnType,
                ColumnName = columnName
            };

            // Assert
            Assert.AreEqual(id, column.Id);
            Assert.AreEqual(tableId, column.TableId);
            Assert.AreEqual(rank, column.Rank);
            Assert.AreEqual(columnType, column.ColumnType);
            Assert.AreEqual(columnName, column.ColumnName);
        }

        [TestMethod]
        public void ColumnModel_Serialization_ShouldWorkCorrectly()
        {
            // Arrange
            var column = new ColumnModel
            {
                Id = Guid.NewGuid(),
                TableId = Guid.NewGuid(),
                Rank = 1,
                ColumnType = (short)ColumnType.Int32,
                ColumnName = "SerializedColumn"
            };

            // Act
            var serialized = MessagePackSerializer.Serialize(column);
            var deserialized = MessagePackSerializer.Deserialize<ColumnModel>(serialized);

            // Assert
            Assert.AreEqual(column.Id, deserialized.Id);
            Assert.AreEqual(column.TableId, deserialized.TableId);
            Assert.AreEqual(column.Rank, deserialized.Rank);
            Assert.AreEqual(column.ColumnType, deserialized.ColumnType);
            Assert.AreEqual(column.ColumnName, deserialized.ColumnName);
        }
    }
}
