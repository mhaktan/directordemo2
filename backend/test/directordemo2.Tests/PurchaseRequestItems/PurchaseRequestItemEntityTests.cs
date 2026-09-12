using System;
using Xunit;
using FluentAssertions;
using directordemo2.Entities;

namespace directordemo2.Tests.PurchaseRequestItems
{
    public class PurchaseRequestItemEntityTests
    {
        [Fact]
        public void PurchaseRequestItem_ShouldBeCreatable()
        {
            // Act
            var entity = new PurchaseRequestItem();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void PurchaseRequestItem_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new PurchaseRequestItem();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void PurchaseRequestItem_ProductName_ShouldAcceptValue()
        {
            var entity = new PurchaseRequestItem { ProductName = "Test Value" };
            entity.ProductName.Should().Be("Test Value");
        }

    }
}
