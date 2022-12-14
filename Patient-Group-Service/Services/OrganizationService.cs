using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrganizationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void Create(Organization organization)
    {
        _unitOfWork.Organizations.Add(organization);
        _unitOfWork.Complete();
    }

    public void Remove(Organization organization)
    {
        _unitOfWork.Organizations.Remove(organization);
        _unitOfWork.Complete();
    }

    public bool Exists(string id)
    {
        return _unitOfWork.Organizations.GetById(id) != null;
    }

    public IEnumerable<Organization> GetAll()
    {
        return _unitOfWork.Organizations.GetAll();
    }
}