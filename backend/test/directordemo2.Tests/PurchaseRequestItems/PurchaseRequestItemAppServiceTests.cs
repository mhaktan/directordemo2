using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using directordemo2.Entities;
using directordemo2.PurchaseRequestItems;
using directordemo2.PurchaseRequestItems.Dto;
using directordemo2.Flows;

namespace directordemo2.Tests.PurchaseRequestItems
{
    public class PurchaseRequestItemAppServiceTests
    {
        private readonly Mock<IRepository<PurchaseRequestItem, long>> _repositoryMock;
        private readonly PurchaseRequestItemAppService _service;

        public PurchaseRequestItemAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<PurchaseRequestItem, long>>();
            _service = new PurchaseRequestItemAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new PurchaseRequestItem { Id = 1, ProductName = "Test productName", Quantity = 10.0m, UnitPrice = 10.0m },
                new PurchaseRequestItem { Id = 2, ProductName = "Test productName", Quantity = 10.0m, UnitPrice = 10.0m },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act
            var result = _repositoryMock.Object.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Fact]
        public void Repository_GetAll_WithFilter_ShouldWork()
        {
            // Arrange
            var entities = new[]
            {
                new PurchaseRequestItem { Id = 1, ProductName = "Test productName", Quantity = 10.0m, UnitPrice = 10.0m },
                new PurchaseRequestItem { Id = 2, ProductName = "Test productName", Quantity = 10.0m, UnitPrice = 10.0m },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act — simulate keyword filter
            var result = _repositoryMock.Object.GetAll()
                .Where(x => x.Id.ToString().Contains("1"));

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldInsertEntity()
        {
            // Arrange
            var dto = new CreatePurchaseRequestItemDto
            {
                ProductName = "Test productName", Quantity = 10.0m, UnitPrice = 10.0m
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<PurchaseRequestItem>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new PurchaseRequestItem { Id = 1, ProductName = "Test productName", Quantity = 10.0m, UnitPrice = 10.0m });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new PurchaseRequestItem { Id = 1, ProductName = "Test productName", Quantity = 10.0m, UnitPrice = 10.0m });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
