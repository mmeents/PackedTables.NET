using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables.Dictionaries;
using PackedTables.Models;

namespace PackedTables.Tests.Dictionaries
{
    [TestClass]
    public class ColumnsTests
    {
        [TestMethod]
        public void Columns_Add_ShouldAddColumn()
        {
            // Arrange
            var columns = new Columns();
            var column = new ColumnModel
            {
                ColumnName = "TestColumn",
                ColumnType = (short)ColumnType.String
            };

            // Act
            var addedColumn = columns.Add(column);

            // Assert
            Assert.IsTrue(columns.Contains(addedColumn.Id));
            Assert.IsTrue(columns.Contains(addedColumn.ColumnName));
            Assert.AreEqual(addedColumn, columns[addedColumn.Id]);
        }

        [TestMethod]
        public void Columns_Remove_ShouldRemoveColumn()
        {
            // Arrange
            var columns = new Columns();
            var column = new ColumnModel
            {
                ColumnName = "TestColumn",
                ColumnType = (short)ColumnType.String
            };
            var addedColumn = columns.Add(column);

            // Act
            columns.Remove(addedColumn.Id);

            // Assert
            Assert.IsFalse(columns.Contains(addedColumn.Id));
            Assert.IsFalse(columns.Contains(addedColumn.ColumnName));
        }

        [TestMethod]
        public void Columns_ByName_ShouldReturnColumn()
        {
            // Arrange
            var columns = new Columns();
            var column = new ColumnModel
            {
                ColumnName = "TestColumn",
                ColumnType = (short)ColumnType.String
            };
            var addedColumn = columns.Add(column);

            // Act
            var retrievedColumn = columns.ByName("TestColumn");

            // Assert
            Assert.IsNotNull(retrievedColumn);
            Assert.AreEqual(addedColumn, retrievedColumn);
        }

        [TestMethod]
        public void Columns_Contains_ShouldReturnTrueForExistingColumn()
        {
            // Arrange
            var columns = new Columns();
            var column = new ColumnModel
            {
                ColumnName = "TestColumn",
                ColumnType = (short)ColumnType.String
            };
            var addedColumn = columns.Add(column);

            // Act
            var containsById = columns.Contains(addedColumn.Id);
            var containsByName = columns.Contains(addedColumn.ColumnName);

            // Assert
            Assert.IsTrue(containsById);
            Assert.IsTrue(containsByName);
        }

        [TestMethod]
        public void Columns_AsList_ShouldReturnOrderedColumns()
        {
            // Arrange
            var columns = new Columns();
            var column1 = new ColumnModel { ColumnName = "Column1", Rank = 2 };
            var column2 = new ColumnModel { ColumnName = "Column2", Rank = 1 };
            columns.Add(column1);
            columns.Add(column2);

            // Act
            var asList = columns.AsList.ToList();

            // Assert
            Assert.AreEqual(2, asList.Count);
            Assert.AreEqual("Column2", asList[0].ColumnName);
            Assert.AreEqual("Column1", asList[1].ColumnName);
        }

        [TestMethod]
        public void Columns_GetNextRank_ShouldReturnCorrectRank()
        {
            // Arrange
            var columns = new Columns();
            var column1 = new ColumnModel { ColumnName = "Column1", Rank = 2 };
            var column2 = new ColumnModel { ColumnName = "Column2", Rank = 1 };
            columns.Add(column1);
            columns.Add(column2);

            // Act
            var nextRank = columns.GetNextRank();

            // Assert
            Assert.AreEqual(3, nextRank);
        }
    }
}
