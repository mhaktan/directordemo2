using System;
using Xunit;
using FluentAssertions;
using directordemo2.Entities;

namespace directordemo2.Tests.PurchaseOrders
{
    public class PurchaseOrderEntityTests
    {
        [Fact]
        public void PurchaseOrder_ShouldBeCreatable()
        {
            // Act
            var entity = new PurchaseOrder();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void PurchaseOrder_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new PurchaseOrder();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void PurchaseOrder_OrderNumber_ShouldAcceptValue()
        {
            var entity = new PurchaseOrder { OrderNumber = "Test Value" };
            entity.OrderNumber.Should().Be("Test Value");
        }

    }
}
