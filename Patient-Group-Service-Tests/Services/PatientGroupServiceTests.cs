using Moq;
using Patient_Group_Service.Events;
using Patient_Group_Service.Exceptions;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;
using Patient_Group_Service.Models.LinkTables;
using Patient_Group_Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Patient_Group_Service_Tests.Services;

public class PatientGroupServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<INatsService> _natsService = new();
    private readonly Mock<IPatientService> _patientService = new();
    private readonly Mock<ICaregiverService> _caregiverService = new();

    public PatientGroupServiceTests()
    {
        var patientGroup = new PatientGroup();
        var patient = new Patient();
        var caregiver = new Caregiver();

        _unitOfWork.Setup(x => x.PatientGroups.Add(patientGroup)).Returns(patientGroup);
        _unitOfWork.Setup(x => x.Patients.Add(patient)).Returns(patient);
        _unitOfWork.Setup(x => x.Caregivers.Add(caregiver)).Returns(caregiver);
        _unitOfWork.Setup(x => x.Complete()).Returns(0);
    }

    [Fact]
    public void Get_ShouldSucceed()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        _unitOfWork.Setup(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id)).Returns(patientGroup);

        // Act
        var result = patientGroupService.Get(patientGroup.Id, organization.Id);

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id), Times.Once);

        Assert.Equal(patientGroup, result);
    }

    [Fact]
    public void Get_NotFoundException()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        _unitOfWork.Setup(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id)).Returns((PatientGroup?)null);

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
            patientGroupService.Get(patientGroup.Id, organization.Id)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id), Times.Once);

        Assert.Equal($"Patient group with id '{patientGroup.Id}' doesn't exist.", result.Message);
    }

    [Fact]
    public void Create_ShouldSucceed()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };


        _unitOfWork.Setup(x => x.Organizations.GetById(organization.Id)).Returns(organization);

        // Act
        var newPatientGroup = patientGroupService.Create(patientGroup.GroupName, patientGroup.Description, organization.Id);

        // Assert
        _unitOfWork.Verify(x => x.Complete(), Times.Once);

        Assert.Equal("group1", patientGroup.GroupName);
        Assert.Equal("description", patientGroup.Description);
        Assert.Single(_natsService.Invocations.Where(i => i.Method.Name == "Publish"));
    }

    [Fact]
    public void Create_ThrowsBadRequestException()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = String.Empty,
            Description = "description"
        };

        var tenantId = "tenant";

        // Act
        var result = Assert.Throws<BadRequestException>(() =>
            patientGroupService.Create(patientGroup.GroupName, patientGroup.Description, tenantId)
        );

        // Assert
        _unitOfWork.Verify(x => x.Complete(), Times.Never);

        Assert.Empty(_natsService.Invocations.Where(i => i.Method.Name == "Publish"));

        Assert.Equal($"Name cannot be empty.", result.Message);
    }

    [Fact]
    public void AddPatient_ShouldSucceed()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        var patient = new Patient
        {
            Id = "test-id",
            FirstName = "firstname",
            LastName = "lastname"
        };

        var organization = new Organization
        {
            Id = "tenant",
        };

        _unitOfWork.Setup(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id)).Returns(patientGroup);
        _patientService.Setup(x => x.Get(patient.Id, organization.Id)).Returns(patient);

        // Act
        patientGroupService.AddPatient(patientGroup.Id, patient.Id, organization.Id);

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.AddPatient(patientGroup, patient), Times.Once);
        _unitOfWork.Verify(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id), Times.Once);
        _patientService.Verify(x => x.Get(patient.Id, organization.Id), Times.Once);

        _unitOfWork.Verify(x => x.PatientGroups.AddPatient(patientGroup, patient));
        _unitOfWork.Verify(x => x.Complete(), Times.Once);

        Assert.Single(_natsService.Invocations.Where(i => i.Method.Name == "Publish"));
    }

    [Fact]
    public void AddPatient_ThrowsNotFoundException()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        var patient = new Patient
        {
            Id = "test-id",
            FirstName = "firstname",
            LastName = "lastname"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
            patientGroupService.AddPatient(patientGroup.Id, patient.Id, organization.Id)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id), Times.Once);

        Assert.Empty(_natsService.Invocations.Where(i => i.Method.Name == "Publish"));
    }

    [Fact]
    public async Task AddCaregiver_ShouldSucceed()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        var caregiver = new Caregiver
        {
            Id = "test-id",
            AzureID = "test-id"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        _unitOfWork.Setup(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id)).Returns(patientGroup);
        _caregiverService.Setup(x => x.Get(caregiver.Id, organization.Id)).Returns(Task.FromResult(caregiver));

        // Act
        await patientGroupService.AddCaregiver(patientGroup.Id, caregiver.Id, organization.Id);

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.AddCaregiver(patientGroup, caregiver), Times.Once);
        _unitOfWork.Verify(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id), Times.Once);
        _caregiverService.Verify(x => x.Get(caregiver.Id, organization.Id), Times.Once);
        _unitOfWork.Verify(x => x.Complete(), Times.Once);

        Assert.Single(_natsService.Invocations.Where(i => i.Method.Name == "Publish"));
    }

    [Fact]
    public async Task AddCaregiver_ThrowsNotFoundException()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        var caregiver = new Caregiver
        {
            Id = "test-id",
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(() =>
            patientGroupService.AddCaregiver(patientGroup.Id, caregiver.Id, organization.Id)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id), Times.Once);
        _unitOfWork.Verify(x => x.Caregivers.GetByAzureIdAndTenant(caregiver.Id, organization.Id), Times.Never);
        _unitOfWork.Verify(x => x.Complete(), Times.Never);

        Assert.Empty(_natsService.Invocations.Where(i => i.Method.Name == "Publish"));

        Assert.Equal($"Patient group with id '{caregiver.Id}' doesn't exist.", result.Message);
    }


    [Fact]
    public void GetPatient_ShouldSucceed()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        _unitOfWork.Setup(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id)).Returns(patientGroup);

        // Act
        patientGroupService.GetPatients(patientGroup.Id, organization.Id);

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id), Times.Once);
    }

    [Fact]
    public void GetPatient_ThrowsNotFound()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);


        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
             patientGroupService.GetPatients(patientGroup.Id, organization.Id)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id), Times.Once);

        Assert.Equal($"Patient group with id '{patientGroup.Id}' doesn't exist.", result.Message);
    }

    [Fact]
    public void GetCaregiver_ShouldSucceed()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        _unitOfWork.Setup(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id)).Returns(patientGroup);

        // Act
        patientGroupService.GetCaregivers(patientGroup.Id, organization.Id);

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id), Times.Once);
    }

    [Fact]
    public void GetCaregiver_ThrowsNotFound()
    {
        // Act
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object, _caregiverService.Object, _patientService.Object);


        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
             patientGroupService.GetCaregivers(patientGroup.Id, organization.Id)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id), Times.Once);

        Assert.Equal($"Patient group with id '{patientGroup.Id}' doesn't exist.", result.Message);
    }

}
