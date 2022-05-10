using Xunit;
using Moq;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;
using Patient_Group_Service.Services;

namespace Patient_Group_Service_Tests.Services;

public class OrganizationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public OrganizationServiceTests()
    {
        var org = new Organization()
        {
            id="69"
        };
        _unitOfWorkMock.Setup(x => x.Organizations.GetById("69")).Returns(org);
    }
    
    [Fact]
    public void OrganizationsExistsShouldSucceed()
    {
        IOrganizationService service = new OrganizationService(_unitOfWorkMock.Object);
        Assert.True(service.Exists("69"));
    }
    
    [Fact]
    public void OrganizationsExistsShouldFail()
    {
        IOrganizationService service = new OrganizationService(_unitOfWorkMock.Object);
        Assert.False(service.Exists("1"));
    }
}