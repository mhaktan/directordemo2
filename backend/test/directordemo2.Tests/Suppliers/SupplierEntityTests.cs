using System;
using Xunit;
using FluentAssertions;
using directordemo2.Entities;

namespace directordemo2.Tests.Suppliers
{
    public class SupplierEntityTests
    {
        [Fact]
        public void Supplier_ShouldBeCreatable()
        {
            // Act
            var entity = new Supplier();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Supplier_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Supplier();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsActive.Should().Be(false);
        }

        [Fact]
        public void Supplier_Code_ShouldAcceptValue()
        {
            var entity = new Supplier { Code = "Test Value" };
            entity.Code.Should().Be("Test Value");
        }

        [Fact]
        public void Supplier_Name_ShouldAcceptValue()
        {
            var entity = new Supplier { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
