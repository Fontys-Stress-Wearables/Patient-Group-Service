using System.Collections.Generic;
using Moq;
using Patient_Group_Service.Exceptions;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;
using Patient_Group_Service.Models.LinkTables;
using Patient_Group_Service.Services;
using Xunit;

namespace Patient_Group_Service_Tests.Services;

public class PatientServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
        
    [Fact]
    public void Get_ShouldSucceed()
    {
        var sut = new PatientService(_unitOfWork.Object);

        var patient = new Patient
        {
            Id = "test-id",
            FirstName = "test-name",
            LastName = "test-lastname"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        _unitOfWork.Setup(x => x.Patients.GetByIdAndTenant(patient.Id, organization.Id)).Returns(patient);
        
        var result = sut.Get(patient.Id, organization.Id);
        
        _unitOfWork.Verify(x => x.Patients.GetByIdAndTenant(patient.Id, organization.Id), Times.Once);

        Assert.Equal(patient, result);
    }
    
    [Fact]
    public void Get_NotFoundException()
    {
        var sut = new PatientService(_unitOfWork.Object);

        var patient = new Patient
        {
            Id = "test-id"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        _unitOfWork.Setup(x => x.Patients.GetByIdAndTenant(patient.Id, organization.Id)).Returns((Patient?) null);
        
        var result = Assert.Throws<NotFoundException>(() =>
            sut.Get(patient.Id, organization.Id)
        );
        
        _unitOfWork.Verify(x => x.Patients.GetByIdAndTenant(patient.Id, organization.Id), Times.Once);
        Assert.Equal($"Patient with id '{patient.Id}' not found.", result.Message);
    }
    
    [Fact]
    public void Create_ShouldSucceed()
    {
        var sut = new PatientService(_unitOfWork.Object);

        var patient = new Patient
        {
            Id = "test-id",
            FirstName = "test-name",
            LastName = "test-lastname"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        _unitOfWork.Setup(x => x.Patients.Add(patient)).Returns(patient);
        _unitOfWork.Setup(x => x.Organizations.GetById(organization.Id)).Returns(organization);

        sut.Create(patient, organization.Id);
        
        _unitOfWork.Verify(x => x.Patients.Add(patient), Times.Once);
        _unitOfWork.Verify(x => x.Complete(), Times.Once);
    }
    
    [Fact]
    public void Create_ThrowsCouldNotCreateException()
    {
        var sut = new PatientService(_unitOfWork.Object);

        var patient = new Patient
        {
            Id = "test-id",
            FirstName = "test-name",
            LastName = "test-lastname"
        };

        var organization = new Organization
        {
            Id = "tenant"
        };

        _unitOfWork.Setup(x => x.Patients.Add(patient)).Returns((Patient?) null);
        _unitOfWork.Setup(x => x.Organizations.GetById(organization.Id)).Returns(organization);

        var result = Assert.Throws<CouldNotCreateException>(() =>
            sut.Create(patient, organization.Id)
        );        
        
        Assert.Equal($"Could not create patient with id '{patient.Id}'.", result.Message);
        _unitOfWork.Verify(x => x.Complete(), Times.Never);
    }
}