using Moq;
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
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        _unitOfWork.Setup(x => x.PatientGroups.GetById(patientGroup.Id)).Returns(patientGroup);

        // Act
        var result = patientGroupService.Get(patientGroup.Id);

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);

        Assert.Equal(patientGroup, result);
    }

    [Fact]
    public void Get_NotFoundException()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        _unitOfWork.Setup(x => x.PatientGroups.GetById(patientGroup.Id)).Returns((PatientGroup?)null);

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
            patientGroupService.Get(patientGroup.Id)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);

        Assert.Equal($"Patient group with id '{patientGroup.Id}' doesn't exist.", result.Message);
    }

    [Fact]
    public void Create_ShouldSucceed()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        // Act
        var newPatientGroup = patientGroupService.Create(patientGroup.GroupName, patientGroup.Description);

        // Assert
        _unitOfWork.Verify(x => x.Complete(), Times.Once);

        _natsService.Verify(x => x.Publish("patient-group-created", newPatientGroup), Times.Once);

        Assert.Equal("group1", patientGroup.GroupName);
        Assert.Equal("description", patientGroup.Description);

    }

    [Fact]
    public void Create_ThrowsBadRequestException()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = String.Empty,
            Description = "description"
        };

        // Act
        var result = Assert.Throws<BadRequestException>(() =>
            patientGroupService.Create(patientGroup.GroupName, patientGroup.Description)
        );

        // Assert
        _unitOfWork.Verify(x => x.Complete(), Times.Never);

        Assert.Empty(_natsService.Invocations);

        Assert.Equal($"Name cannot be empty.", result.Message);
    }

    [Fact]
    public void AddPatient_ShouldSucceed()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

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

        _unitOfWork.Setup(x => x.PatientGroups.GetById(patientGroup.Id)).Returns(patientGroup);
        _unitOfWork.Setup(x => x.Patients.GetById(patient.Id)).Returns(patient);

        // Act
        patientGroupService.AddPatient(patientGroup.Id, patient.Id);

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.AddPatient(patientGroup, patient), Times.Once);
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);
        _unitOfWork.Verify(x => x.Patients.GetById(patient.Id), Times.Once);
        _unitOfWork.Verify(x => x.Complete(), Times.Once);

        // Moq verify doesn't seem to work with anonymous types
        // For now this seems like the easiest way to check whether the a method on the NATS service has been called
        Assert.Single(_natsService.Invocations);
    }

    [Fact]
    public void AddPatient_ThrowsNotFoundException()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

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

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
            patientGroupService.AddPatient(patientGroup.Id, patient.Id)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);

        Assert.Empty(_natsService.Invocations);
    }

    [Fact]
    public void AddPatient_ThrowsBadRequestException()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

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

        _unitOfWork.Setup(x => x.PatientGroups.GetById(patientGroup.Id)).Returns(patientGroup);

        // Act
        var result = Assert.Throws<BadRequestException>(() =>
            patientGroupService.AddPatient(patientGroup.Id, patient.Id)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);
        _unitOfWork.Verify(x => x.Patients.GetById(patient.Id), Times.Once);
        _unitOfWork.Verify(x => x.Complete(), Times.Never);

        Assert.Empty(_natsService.Invocations);

        Assert.Equal($"Patient with id '{patient.Id}' doesn't exist.", result.Message);
    }

    [Fact]
    public void AddCaregiver_ShouldSucceed()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

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

        _unitOfWork.Setup(x => x.PatientGroups.GetById(patientGroup.Id)).Returns(patientGroup);
        _unitOfWork.Setup(x => x.Caregivers.GetById(caregiver.Id)).Returns(caregiver);

        // Act
        patientGroupService.AddCaregiver(patientGroup.Id, caregiver.Id, TODO);

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.AddCaregiver(patientGroup, caregiver), Times.Once);
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);
        _unitOfWork.Verify(x => x.Caregivers.GetById(caregiver.Id), Times.Once);
        _unitOfWork.Verify(x => x.Complete(), Times.Once);

        Assert.Single(_natsService.Invocations);
    }

    [Fact]
    public void AddCaregiver_ThrowsNotFoundException()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

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

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
            patientGroupService.AddCaregiver(patientGroup.Id, caregiver.Id, TODO)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);
        _unitOfWork.Verify(x => x.Caregivers.GetById(caregiver.Id), Times.Never);
        _unitOfWork.Verify(x => x.Complete(), Times.Never);

        Assert.Empty(_natsService.Invocations);

        Assert.Equal($"Patient group with id '{caregiver.Id}' doesn't exist.", result.Message);
    }

    [Fact]
    public void AddCaregiver_ThrowsBadRequestException()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

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

        _unitOfWork.Setup(x => x.PatientGroups.GetById(patientGroup.Id)).Returns(patientGroup);

        // Act
        var result = Assert.Throws<BadRequestException>(() =>
            patientGroupService.AddCaregiver(patientGroup.Id, caregiver.Id, TODO)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);
        _unitOfWork.Verify(x => x.Caregivers.GetById(caregiver.Id), Times.Once);
        _unitOfWork.Verify(x => x.Complete(), Times.Never);

        Assert.Empty(_natsService.Invocations);

        Assert.Equal($"Caregiver with id '{caregiver.Id}' doesn't exist.", result.Message);
    }


    [Fact]
    public void GetPatient_ShouldSucceed()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        _unitOfWork.Setup(x => x.PatientGroups.GetById(patientGroup.Id)).Returns(patientGroup);

        // Act
        patientGroupService.GetPatients(patientGroup.Id);

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);
    }

    [Fact]
    public void GetPatient_ThrowsNotFound()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);


        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
             patientGroupService.GetPatients(patientGroup.Id)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);

        Assert.Equal($"Patient group with id '{patientGroup.Id}' doesn't exist.", result.Message);
    }

    [Fact]
    public void GetCaregiver_ShouldSucceed()
    {
        // Arrange
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);

        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        _unitOfWork.Setup(x => x.PatientGroups.GetById(patientGroup.Id)).Returns(patientGroup);

        // Act
        patientGroupService.GetCaregivers(patientGroup.Id);

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);
    }

    [Fact]
    public void GetCaregiver_ThrowsNotFound()
    {
        // Act
        var patientGroupService = new PatientGroupService(_unitOfWork.Object, _natsService.Object);


        var patientGroup = new PatientGroup
        {
            Id = "test-id",
            GroupName = "group1",
            Description = "description"
        };

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
             patientGroupService.GetCaregivers(patientGroup.Id)
        );

        // Assert
        _unitOfWork.Verify(x => x.PatientGroups.GetById(patientGroup.Id), Times.Once);

        Assert.Equal($"Patient group with id '{patientGroup.Id}' doesn't exist.", result.Message);
    }

}
