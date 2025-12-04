using Moq;
using PlumbingBiddingTool.Application.Contractors;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;

namespace PlumbingBiddingTool.Application.Tests;

public class ContractorServiceTests
{
    private readonly Mock<IContractorRepository> _mockRepository;
    private readonly ContractorService _service;

    public ContractorServiceTests()
    {
        _mockRepository = new Mock<IContractorRepository>();
        _service = new ContractorService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllContractorsAsync_ReturnsAllContractors()
    {
        // Arrange
        var contractors = new List<Contractor>
        {
            new Contractor { Id = 1, Name = "ABC Plumbing" },
            new Contractor { Id = 2, Name = "XYZ Contractors" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(contractors);

        // Act
        var result = await _service.GetAllContractorsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetContractorByIdAsync_ReturnsContractor_WhenExists()
    {
        // Arrange
        var contractor = new Contractor { Id = 1, Name = "ABC Plumbing" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contractor);

        // Act
        var result = await _service.GetContractorByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("ABC Plumbing", result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetContractorByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Contractor?)null);

        // Act
        var result = await _service.GetContractorByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task CreateContractorAsync_CreatesAndReturnsContractor()
    {
        // Arrange
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Contractor>()))
            .ReturnsAsync((Contractor c) => { c.Id = 1; return c; });

        // Act
        var result = await _service.CreateContractorAsync("New Contractor");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Contractor", result.Name);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Contractor>()), Times.Once);
    }

    [Fact]
    public async Task UpdateContractorAsync_CallsRepositoryUpdate()
    {
        // Arrange
        var contractor = new Contractor { Id = 1, Name = "Updated Contractor" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contractor);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Contractor>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateContractorAsync(1, "Updated Contractor");

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Contractor>(c => c.Id == 1 && c.Name == "Updated Contractor")), Times.Once);
    }

    [Fact]
    public async Task DeleteContractorAsync_CallsRepositoryDelete()
    {
        // Arrange
        var contractorId = 1;
        _mockRepository.Setup(r => r.DeleteAsync(contractorId)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteContractorAsync(contractorId);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(contractorId), Times.Once);
    }
}
