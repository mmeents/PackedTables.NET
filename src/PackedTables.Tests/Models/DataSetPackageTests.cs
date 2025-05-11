using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessagePack;
using PackedTables.Models;

namespace PackedTables.Tests.Models
{
    [TestClass]
    public class DataSetPackageTests
    {
        [TestMethod]
        public void DataSetPackage_Rows_DefaultValue_ShouldBeEmpty()
        {
            // Arrange
            var dataSetPackage = new DataSetPackage();

            // Act
            var rows = dataSetPackage.Rows;

            // Assert
            Assert.IsNotNull(rows);
            Assert.AreEqual(0, rows.Count());
        }

        [TestMethod]
        public void DataSetPackage_Rows_ShouldAllowSettingAndGetting()
        {
            // Arrange
            var dataSetPackage = new DataSetPackage();
            var row1 = new RowModel { Id = Guid.NewGuid(), TableId = Guid.NewGuid() };
            var row2 = new RowModel { Id = Guid.NewGuid(), TableId = Guid.NewGuid() };
            var rows = new List<RowModel> { row1, row2 };

            // Act
            dataSetPackage.Rows = rows;

            // Assert
            Assert.AreEqual(2, dataSetPackage.Rows.Count());
            Assert.IsTrue(dataSetPackage.Rows.Contains(row1));
            Assert.IsTrue(dataSetPackage.Rows.Contains(row2));
        }

        [TestMethod]
        public void DataSetPackage_Rows_Serialization_ShouldWorkCorrectly()
        {
            // Arrange
            var row1 = new RowModel { Id = Guid.NewGuid(), TableId = Guid.NewGuid() };
            var row2 = new RowModel { Id = Guid.NewGuid(), TableId = Guid.NewGuid() };
            var dataSetPackage = new DataSetPackage
            {
                Rows = new List<RowModel> { row1, row2 }
            };

            // Act
            var serialized = MessagePackSerializer.Serialize(dataSetPackage);
            var deserialized = MessagePackSerializer.Deserialize<DataSetPackage>(serialized);

            // Assert
            Assert.IsNotNull(deserialized.Rows);
            Assert.AreEqual(2, deserialized.Rows.Count());
            Assert.IsTrue(deserialized.Rows.Any(r => r.Id == row1.Id));
            Assert.IsTrue(deserialized.Rows.Any(r => r.Id == row2.Id));
        }
    }
}
