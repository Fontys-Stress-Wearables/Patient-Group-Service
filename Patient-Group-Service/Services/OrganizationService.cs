using Patient_Group_Service.Interfaces;

namespace Patient_Group_Service.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrganizationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public bool Exists(string id)
    {
        return _unitOfWork.Organizations.GetById(id) != null;
    }
}