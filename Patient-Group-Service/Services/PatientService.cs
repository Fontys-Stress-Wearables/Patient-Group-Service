using Patient_Group_Service.Exceptions;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Services;

public class PatientService : IPatientService
{
    private readonly IUnitOfWork _unitOfWork;    
    public PatientService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void Update(Patient patient, string tenantId)
    {
        var org = _unitOfWork.Organizations.GetById(tenantId);
        
        if (org == null)
        {
            throw new NotFoundException($"Organization with id '{tenantId}' doesn't exist.");
        }
        
        patient.Organization = org;
        
        var update = _unitOfWork.Patients.Update(patient);
            
        if(update == null)
        {
            throw new CouldNotCreateException($"Could not update patient with id '{patient.Id}'.");
        }
            
        _unitOfWork.Complete();
    }

    public void Create(Patient patient, string tenantId)
    {
        var org = _unitOfWork.Organizations.GetById(tenantId);
        
        if (org == null)
        {
            throw new NotFoundException($"Organization with id '{tenantId}' doesn't exist.");
        }
        
        patient.Organization = org;
        
        var added = _unitOfWork.Patients.Add(patient);
            
        if(added == null)
        {
            throw new CouldNotCreateException($"Could not create patient with id '{patient.Id}'.");
        }
            
        _unitOfWork.Complete();
    }

    public Patient Get(string id, string tenantId)
    {
        var patient = _unitOfWork.Patients.GetByIdAndTenant(id, tenantId);
            
        if(patient == null)
        {
            throw new NotFoundException($"Patient with id '{id}' not found.");
        }

        return patient;
    }
}