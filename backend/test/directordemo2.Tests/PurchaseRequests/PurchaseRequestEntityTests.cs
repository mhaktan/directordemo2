using System;
using Xunit;
using FluentAssertions;
using directordemo2.Entities;

namespace directordemo2.Tests.PurchaseRequests
{
    public class PurchaseRequestEntityTests
    {
        [Fact]
        public void PurchaseRequest_ShouldBeCreatable()
        {
            // Act
            var entity = new PurchaseRequest();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void PurchaseRequest_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new PurchaseRequest();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void PurchaseRequest_RequestNumber_ShouldAcceptValue()
        {
            var entity = new PurchaseRequest { RequestNumber = "Test Value" };
            entity.RequestNumber.Should().Be("Test Value");
        }

        [Fact]
        public void PurchaseRequest_Justification_ShouldAcceptValue()
        {
            var entity = new PurchaseRequest { Justification = "Test Value" };
            entity.Justification.Should().Be("Test Value");
        }

    }
}
