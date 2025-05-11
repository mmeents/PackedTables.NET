using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackedTables.Models;
using PackedTables.Extensions;

namespace PackedTables.Tests.Models
{
    [TestClass]
    public class FieldModelTests
    {
        [TestMethod]
        public void FieldModel_DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var field = new FieldModel();

            // Assert
            Assert.AreEqual(Guid.Empty, field.Id);
            Assert.AreEqual(Guid.Empty, field.RowId);
            Assert.AreEqual(Guid.Empty, field.ColumnId);
            Assert.AreEqual(string.Empty, field.ValueString);
            Assert.AreEqual(ColumnType.Null, field.ValueType);
        }

        [TestMethod]
        public void FieldModel_Value_Get_ShouldReturnCorrectObject()
        {
            // Arrange
            var field = new FieldModel
            {
                ValueString = "123",
                ValueType = ColumnType.Int32
            };

            // Act
            var value = field.Value;

            // Assert
            Assert.AreEqual(123, value); // Assuming AsObject converts ValueString to the correct type
        }

        [TestMethod]
        public void FieldModel_Value_Set_ShouldUpdateValueString()
        {
            // Arrange
            var field = new FieldModel(){ ValueType = ColumnType.Int32};

            // Act
            field.Value = 123;

            // Assert
            Assert.AreEqual("123", field.ValueString); // Assuming FromObject converts the value to a string
        }

        [TestMethod]
        public void FieldModel_Value_Set_ShouldUpdateValueType()
        {
            // Arrange
            var field = new FieldModel(){ ValueType = ColumnType.Int32 }; 

            // Act
            field.Value = 123;

            // Assert
            Assert.AreEqual(ColumnType.Int32, field.ValueType); // Assuming FromObject sets the correct ValueType
        }
    }
}
