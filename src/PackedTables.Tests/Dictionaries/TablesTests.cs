using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables.Dictionaries;
using PackedTables.Models;

namespace PackedTables.Tests.Dictionaries
{
    [TestClass]
    public class TablesTests
    {
        private PackedTables _packedTables;
        private Tables _tables;

        public TablesTests()
        {
            // Initialize mock PackedTables
            _packedTables = new PackedTables();

            // Initialize Tables with PackedTables owner
            _tables = new Tables(_packedTables);
        }

        [TestMethod]
        public void Tables_Add_ShouldAddTable()
        {
            // Arrange
            var table = new TableModel
            {
                Id = Guid.NewGuid(),
                Name = "TestTable"
            };

            // Act
            var addedTable = _tables.Add(table);

            // Assert
            Assert.IsTrue(_tables.ContainsKey(addedTable.Id));
            Assert.AreEqual(_packedTables, addedTable.Owner.Owner);
        }

        [TestMethod]
        public void Tables_Remove_ById_ShouldRemoveTable()
        {
            // Arrange
            var table = new TableModel
            {
                Id = Guid.NewGuid(),
                Name = "TestTable"
            };
            var addedTable = _tables.Add(table);

            // Act
            _tables.Remove(addedTable.Id);

            // Assert
            Assert.IsFalse(_tables.ContainsKey(addedTable.Id));
        }

        [TestMethod]
        public void Tables_Remove_ByTable_ShouldRemoveTable()
        {
            // Arrange
            var table = new TableModel
            {
                Id = Guid.NewGuid(),
                Name = "TestTable"
            };
            var addedTable = _tables.Add(table);

            // Act
            _tables.Remove(addedTable);

            // Assert
            Assert.IsFalse(_tables.ContainsKey(addedTable.Id));
        }

        [TestMethod]
        public void Tables_AsList_ShouldReturnAllTables()
        {
            // Arrange
            var table1 = new TableModel { Id = Guid.NewGuid(), Name = "Table1" };
            var table2 = new TableModel { Id = Guid.NewGuid(), Name = "Table2" };
            _tables.Add(table1);
            _tables.Add(table2);

            // Act
            var asList = _tables.AsList;

            // Assert
            Assert.AreEqual(2, asList.Count());
            Assert.IsTrue(asList.Contains(table1));
            Assert.IsTrue(asList.Contains(table2));
        }

        [TestMethod]
        public void Tables_AsList_Set_ShouldAddTables()
        {
            // Arrange
            var table1 = new TableModel { Id = Guid.NewGuid(), Name = "Table1" };
            var table2 = new TableModel { Id = Guid.NewGuid(), Name = "Table2" };
            var tableList = new List<TableModel> { table1, table2 };

            // Act
            _tables.AsList = tableList;

            // Assert
            Assert.AreEqual(2, _tables.Count);
            Assert.IsTrue(_tables.Values.Contains(table1));
            Assert.IsTrue(_tables.Values.Contains(table2));
        }

        [TestMethod]
        public void Tables_Indexer_Get_ShouldReturnTable()
        {
            // Arrange
            var table = new TableModel { Id = Guid.NewGuid(), Name = "TestTable" };
            var addedTable = _tables.Add(table);

            // Act
            var retrievedTable = _tables[addedTable.Id];

            // Assert
            Assert.AreEqual(addedTable, retrievedTable);
        }

        [TestMethod]
        public void Tables_Indexer_Set_ShouldAddOrUpdateTable()
        {
            // Arrange
            var table = new TableModel { Id = Guid.NewGuid(), Name = "TestTable" };

            // Act
            _tables[table.Id] = table;

            // Assert
            Assert.IsTrue(_tables.ContainsKey(table.Id));
            Assert.AreEqual(table, _tables[table.Id]);
        }
    }
}
