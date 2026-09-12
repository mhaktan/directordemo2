using System;
using Xunit;
using FluentAssertions;
using directordemo2.Entities;

namespace directordemo2.Tests.Quotations
{
    public class QuotationEntityTests
    {
        [Fact]
        public void Quotation_ShouldBeCreatable()
        {
            // Act
            var entity = new Quotation();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Quotation_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Quotation();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsSelected.Should().Be(false);
        }


    }
}
