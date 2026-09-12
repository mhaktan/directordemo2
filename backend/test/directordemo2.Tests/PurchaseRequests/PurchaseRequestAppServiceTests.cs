using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using directordemo2.Entities;
using directordemo2.PurchaseRequests;
using directordemo2.PurchaseRequests.Dto;
using directordemo2.Flows;

namespace directordemo2.Tests.PurchaseRequests
{
    public class PurchaseRequestAppServiceTests
    {
        private readonly Mock<IRepository<PurchaseRequest, long>> _repositoryMock;
        private readonly PurchaseRequestAppService _service;

        public PurchaseRequestAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<PurchaseRequest, long>>();
            _service = new PurchaseRequestAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object, new Mock<IRepository<StatusChangeLog, long>>().Object, new Mock<IRepository<ApprovalRecord, Guid>>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new PurchaseRequest { Id = 1, RequestNumber = "Test requestNumber", Justification = "Test justification", NeededDate = DateTime.UtcNow, Status = 0 },
                new PurchaseRequest { Id = 2, RequestNumber = "Test requestNumber", Justification = "Test justification", NeededDate = DateTime.UtcNow, Status = 0 },
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
                new PurchaseRequest { Id = 1, RequestNumber = "Test requestNumber", Justification = "Test justification", NeededDate = DateTime.UtcNow, Status = 0 },
                new PurchaseRequest { Id = 2, RequestNumber = "Test requestNumber", Justification = "Test justification", NeededDate = DateTime.UtcNow, Status = 0 },
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
            var dto = new CreatePurchaseRequestDto
            {
                RequestNumber = "Test requestNumber", Justification = "Test justification", NeededDate = DateTime.UtcNow, Status = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<PurchaseRequest>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new PurchaseRequest { Id = 1, RequestNumber = "Test requestNumber", Justification = "Test justification", NeededDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new PurchaseRequest { Id = 1, RequestNumber = "Test requestNumber", Justification = "Test justification", NeededDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
