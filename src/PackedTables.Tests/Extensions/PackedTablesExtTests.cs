using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables;
using PackedTables.Extensions;
using PackedTables.Models;

namespace PackedTables.Tests.Extensions
{
    [TestClass]
    public class PackedTablesExtTests
    {
        private PackedTables _packedTables;

        [TestInitialize]
        public void Setup()
        {
            // Initialize PackedTables with a mock DataSetPackage
            _packedTables = new PackedTables();
            var tableId = Guid.NewGuid();
            var rowId = Guid.NewGuid();

            // Add mock data to the DataSetPackage
            _packedTables.Package.Columns = new[]
            {
                new ColumnModel { Id = Guid.NewGuid(), TableId= tableId, ColumnName = "Column1", ColumnType = (short)ColumnType.String },
                new ColumnModel { Id = Guid.NewGuid(), TableId= tableId, ColumnName = "Column2", ColumnType = (short)ColumnType.Int32 }
            };
            _packedTables.Package.Fields = new[]
            {
                new FieldModel { Id = Guid.NewGuid(), RowId = rowId, ColumnId = _packedTables.Package.Columns.First().Id, ValueString = "TestValue" }
            };
            _packedTables.Package.Rows = new[]
            {
                new RowModel { Id = rowId, TableId = tableId }
            };
            _packedTables.Package.Tables = new[]
            {
                new TableModel { Id = tableId, Name = "TestTable" }
            };
        }

        [TestMethod]
        public void SaveToBase64String_ShouldReturnValidBase64String()
        {
            // Act
            var base64 = _packedTables.SaveToBase64String();

            // Assert
            Assert.IsNotNull(base64);
            Assert.IsTrue(base64.Length > 0);
        }

        [TestMethod]
        public void LoadFromBase64String_ShouldLoadCorrectly()
        {
            // Arrange
            var base64 = _packedTables.SaveToBase64String();
            var newPackedTables = new PackedTables();

            // Act
            newPackedTables.LoadFromBase64String(base64);

            // Assert
            Assert.AreEqual(_packedTables.Package.Columns.Count(), newPackedTables.Package.Columns.Count());
            Assert.AreEqual(_packedTables.Package.Fields.Count(), newPackedTables.Package.Fields.Count());
            Assert.AreEqual(_packedTables.Package.Rows.Count(), newPackedTables.Package.Rows.Count());
            Assert.AreEqual(_packedTables.Package.Tables.Count(), newPackedTables.Package.Tables.Count());
        }

        [TestMethod]
        public void SaveToJson_ShouldReturnValidJsonString()
        {
            // Act
            var json = _packedTables.SaveToJson();

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.StartsWith("[") && json.EndsWith("]"));
        }

        [TestMethod]
        public void LoadFromJson_ShouldLoadCorrectly()
        {
            // Arrange
            var json = _packedTables.SaveToJson();
            var newPackedTables = new PackedTables();

            // Act
            newPackedTables.LoadFromJson(json);

            // Assert
            Assert.AreEqual(_packedTables.Package.Columns.Count(), newPackedTables.Package.Columns.Count());
            Assert.AreEqual(_packedTables.Package.Fields.Count(), newPackedTables.Package.Fields.Count());
            Assert.AreEqual(_packedTables.Package.Rows.Count(), newPackedTables.Package.Rows.Count());
            Assert.AreEqual(_packedTables.Package.Tables.Count(), newPackedTables.Package.Tables.Count());
        }

        [TestMethod]
        public void SaveToFile_ShouldCreateFile()
        {
            // Arrange
            var fileName = Path.GetTempFileName();

            try
            {
                // Act
                _packedTables.SaveToFile(fileName);

                // Assert
                Assert.IsTrue(File.Exists(fileName));
                Assert.IsTrue(new FileInfo(fileName).Length > 0);
            }
            finally
            {
                // Cleanup
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [TestMethod]
        public void LoadFromFile_ShouldLoadCorrectly()
        {
            // Arrange
            var fileName = Path.GetTempFileName();
            _packedTables.SaveToFile(fileName);
            var newPackedTables = new PackedTables();

            try
            {
                // Act
                newPackedTables.LoadFromFile(fileName);

                // Assert
                Assert.AreEqual(_packedTables.Package.Columns.Count(), newPackedTables.Package.Columns.Count());
                Assert.AreEqual(_packedTables.Package.Fields.Count(), newPackedTables.Package.Fields.Count());
                Assert.AreEqual(_packedTables.Package.Rows.Count(), newPackedTables.Package.Rows.Count());
                Assert.AreEqual(_packedTables.Package.Tables.Count(), newPackedTables.Package.Tables.Count());
            }
            finally
            {
                // Cleanup
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }
    }
}
